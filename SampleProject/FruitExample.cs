using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Sets;
using FuzzyFramework.Graphics;
using FuzzyFramework;
using FuzzyFramework.Defuzzification;

namespace SampleProject
{
    public partial class FruitExample : Form
    {

        #region Definition of dimensions
        protected static IDiscreteDimension product;
        protected static IContinuousDimension price;
        protected static IContinuousDimension action;
        #endregion

        #region Definition of members for discrete fuzzy set
        protected static Fruit apple;
        protected static Fruit pear;
        protected static Fruit tomato;
        protected static Fruit blueberry;
        protected static Fruit blackberry;
        protected static Fruit blackCurrant;
        protected static Fruit strawberry;
        protected static Fruit lemon;
        protected static Fruit melon;
        protected static Fruit broccoli;
        #endregion

        #region Basic single-dimensional fuzzy sets
        public static DiscreteSet fruits;
        public static ContinuousSet cheap;
        public static ContinuousSet buyIt;
        #endregion

        #region Internal properties
        protected string _expression = null;
        protected FuzzyRelation _relation;
        protected DefuzzificationFactory.DefuzzificationMethod _defuzzMethod = DefuzzificationFactory.DefuzzificationMethod.RightOfMaximum;
        protected Defuzzification _defuzzification;
        protected bool _ready = false;
        protected bool _waitingForBuild = false;
        protected bool _building = false;
        #endregion

        #region Constructor
        public FruitExample()
        {

            
            InitializeComponent();

            ddlProduct.SelectedIndex = 0;
            ddlDefuzMethod.SelectedIndex = 0;
            buildFruitsSet();
            buildCheapSet();
            buildBuyItSet();
            _ready = true;
            buildRelationNow(true);
        }
        #endregion

        #region Event handlers
        private void trackBarFruit_ValueChanged(object sender, EventArgs e)
        {
            buildFruitsSet();
            buildRelation();
        }

        private void trackBarCheap_ValueChanged(object sender, EventArgs e)
        {
            buildCheapSet();
            buildRelation();
        }

        private void trackBarBuyIt_ValueChanged(object sender, EventArgs e)
        {
            buildBuyItSet();
            buildRelation();
        }

        private void inputControls_ValueChanged(object sender, EventArgs e)
        {
            buildRelation();
        }

        private void timerBuildRelation_Tick(object sender, EventArgs e)
        {
            if (_waitingForBuild && !_building)
                buildRelationNow(false);
        }
        #endregion

        #region Builiding the overall relation from input values, refresing the form content immediatelly

        protected void buildFruitsSet()
        {
            product = new DiscreteDimension("Product", "Product being offered");
            fruits = new DiscreteSet(product, "Fruits");
            product.DefaultSet = fruits;

            apple = new Fruit(product, "Apple");
            pear = new Fruit(product, "Pear");
            tomato = new Fruit(product, "Tomato");
            blueberry = new Fruit(product, "Blueberry");
            blackberry = new Fruit(product, "Blackberry");
            blackCurrant = new Fruit(product, "Black-currant");
            strawberry = new Fruit(product, "Strawberry");
            lemon = new Fruit(product, "Lemon");
            melon = new Fruit(product, "Melon");
            broccoli = new Fruit(product, "Broccoli");

            fruits.AddMember(apple, (double)trackBar1.Value / 100);
            fruits.AddMember(pear, (double)trackBar2.Value / 100);
            fruits.AddMember(lemon, (double)trackBar3.Value / 100);
            fruits.AddMember(melon, (double)trackBar4.Value / 100);
            fruits.AddMember(tomato, (double)trackBar5.Value / 100);
            fruits.AddMember(blackCurrant, (double)trackBar6.Value / 100);
            fruits.AddMember(blackberry, (double)trackBar7.Value / 100);
            fruits.AddMember(strawberry, (double)trackBar8.Value / 100);
            fruits.AddMember(blueberry, (double)trackBar9.Value / 100);
            fruits.AddMember(broccoli, (double)trackBar10.Value / 100);

            RelationImage imgFruits = new RelationImage(fruits);
            Bitmap bmpFruits = new Bitmap(pictureBoxFruits.Width, pictureBoxFruits.Height);
            imgFruits.DrawImage(Graphics.FromImage(bmpFruits));
            pictureBoxFruits.Image = bmpFruits;
        }

        protected void buildCheapSet()
        {
            price = new ContinuousDimension("Price", "Price we are about to pay for an offer", "$", 0, 1000);

            if (trackBarCheapSupport.Value < trackBarCheapKernel.Value)
                trackBarCheapSupport.Value = trackBarCheapKernel.Value;

            cheap = new RightLinearSet(price, "Cheap", trackBarCheapKernel.Value, trackBarCheapSupport.Value);
            
            RelationImage imgCheap = new RelationImage(cheap);
            Bitmap bmpCheap = new Bitmap(pictureBoxCheap.Width, pictureBoxCheap.Height);
            imgCheap.DrawImage(Graphics.FromImage(bmpCheap));
            pictureBoxCheap.Image = bmpCheap;
        }

        protected void buildBuyItSet()
        {
            action = new ContinuousDimension("Action", "-10 surely don't buy ... +10 surely buy", "", -10, +10);

            if (trackBarBuyItKernel.Value < -8)
                trackBarBuyItKernel.Value = -8;
            if (trackBarBuyItCrossover.Value < -9)
                trackBarBuyItCrossover.Value = -9;
            
            if (trackBarBuyItCrossover.Value > trackBarBuyItKernel.Value-1)
                trackBarBuyItCrossover.Value = trackBarBuyItKernel.Value - 1;

            if (trackBarBuyItSupport.Value > trackBarBuyItCrossover.Value - 1)
                trackBarBuyItSupport.Value = trackBarBuyItCrossover.Value - 1;

            buyIt = new LeftQuadraticSet  (action, "Buy it!", trackBarBuyItSupport.Value, trackBarBuyItCrossover.Value, trackBarBuyItKernel.Value);

            RelationImage imgBuyIt = new RelationImage(buyIt);
            Bitmap bmpBuyIt = new Bitmap(pictureBoxBuyIt.Width, pictureBoxBuyIt.Height);
            imgBuyIt.DrawImage(Graphics.FromImage(bmpBuyIt));
            pictureBoxBuyIt.Image = bmpBuyIt;

        }

        protected void buildRelation()
        {
            //refresh of the treeview and the graphs is time consuming. We will only do it every second. See the timer component.
            _waitingForBuild = true;
        }


        protected void buildRelationNow(bool initial)
        {
            if (!_ready)
                return;

            _waitingForBuild = false;
            _building = true;

            bool _expressionChanged = false;

            decimal inputProduct = ddlProduct.SelectedIndex + 1;
            decimal inputPrice = txtPrice.Value;

            #region Realtime expression evaluation by means of C# parser
            string strExpression = txtExpression.Text;
            prependFullName(ref strExpression, "cheap");
            prependFullName(ref strExpression, "fruits");
            prependFullName(ref strExpression, "buyIt");

            object obj = Evaluator.Eval(strExpression);

            if (obj != null)
            {
                if (!(obj is FuzzyRelation))
                {
                    MessageBox.Show(String.Format("ERROR: Object of type FuzzyRelation expected as the result of the expression.\r\nThis object is type {0}.", obj.GetType().FullName),
                        "Error evaluating expression", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    _relation = (FuzzyRelation)obj;
                    if (_expression != txtExpression.Text)
                        _expressionChanged = true;
                    _expression = txtExpression.Text;

                }
            }
            #endregion

            #region Defuzzification
            DefuzzificationFactory.DefuzzificationMethod method = (DefuzzificationFactory.DefuzzificationMethod)ddlDefuzMethod.SelectedIndex;

            _defuzzification = DefuzzificationFactory.GetDefuzzification(
                _relation,
                new Dictionary<IDimension, decimal> {
                        { product, inputProduct },
                        { price, inputPrice }
                },
                method
                );

            _defuzzMethod = method;
            #endregion

            #region Output value
            string unit = ((IContinuousDimension)_defuzzification.OutputDimension).Unit;
            lblOutput.Text = _defuzzification.CrispValue.ToString("F5") + (string.IsNullOrEmpty(unit) ? "" : " " + unit);
            #endregion

            Cursor.Current = Cursors.WaitCursor;

            #region storing TreeView selection
            //Store information about currenlty selected node. It will become handy
            //when selecting the same node after the refresh (if applicable)
            List<int> selectedNodePath = new List<int>();

            if (treeViewRelation.SelectedNode != null)
            {
                TreeNode pointer = treeViewRelation.SelectedNode;
                while (pointer != null)
                {
                    selectedNodePath.Add(pointer.Index);
                    pointer = pointer.Parent;
                }
            }
            else if (initial)
            {
                selectedNodePath.Add(0);
            }
            #endregion

            TreeSource ts = new TreeSource(_defuzzification);
            ts.DrawImageOnNodeSelect = false;
            ts.BuildTree(treeViewRelation, pictureBoxGraph, lblGraphCaption);
            //Cursor.Current = Cursors.Default;

            #region restoring TreeView selection
            if ((!_expressionChanged || initial) && selectedNodePath.Count() > 0 && selectedNodePath[selectedNodePath.Count()-1] < treeViewRelation.Nodes.Count)
            {
                //We will now try to restore the selection
                TreeNode pointer = treeViewRelation.Nodes[selectedNodePath[selectedNodePath.Count() - 1]];

                for (int i = selectedNodePath.Count() - 2; i >= 0; i--)
                {
                    if (selectedNodePath[i] >= pointer.Nodes.Count)
                    {
                        pointer = null;
                        break;
                    }
                    pointer = pointer.Nodes[selectedNodePath[i]];
                }

                if (pointer != null)
                {
                    treeViewRelation.SelectedNode = pointer;
                    ts.DrawDetailImage(pointer);
                }
            }

            Cursor.Current = Cursors.Default;
            ts.DrawImageOnNodeSelect = true;
            #endregion

            _building = false;
        }

        #endregion

        #region Auxiliary
        protected void prependFullName(ref string expression, string variable)
        {
            string classFullName = this.GetType().FullName;
            expression = expression.Replace(variable, classFullName + "." + variable);
        }

        /// <summary>
        /// Switches to the other example
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSwitch_Click(object sender, EventArgs e)
        {
            TemperatureExample tempertatureForm = new TemperatureExample(this);
            this.Hide();
            tempertatureForm.Show();
        }
        #endregion



    }
}
