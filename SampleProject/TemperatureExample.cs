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
    public partial class TemperatureExample : Form
    {

        #region Definition of dimensions
        protected static IContinuousDimension temperature;
        protected static IContinuousDimension deltaTemperature;
        protected static IContinuousDimension action;
        #endregion

        #region Basic single-dimensional fuzzy sets
        //for t:
        public static ContinuousSet lowTemperature;
        public static ContinuousSet highTemperature;
        public static ContinuousSet correctTemperature;

        //for ∆t:
        public static ContinuousSet risingTemperature;
        public static ContinuousSet fallingTemperature;
        public static ContinuousSet constantTemperature;

        //for action
        public static ContinuousSet heat;
        public static ContinuousSet cool;
        public static ContinuousSet doNothing;

        #endregion

        #region Internal properties
        protected string _expression = null;
        protected FuzzyRelation _relation;
        protected DefuzzificationFactory.DefuzzificationMethod _defuzzMethod = DefuzzificationFactory.DefuzzificationMethod.CenterOfMaximum;
        protected Defuzzification _defuzzification;
        protected bool _ready = false;
        protected bool _waitingForBuild = false;
        protected bool _building = false;
        Form _parentForm;
        #endregion

        #region Constructor
        public TemperatureExample(Form parentForm)
        {
            _parentForm = parentForm;
            
            InitializeComponent();

            ddlDefuzMethod.SelectedIndex = 0;
            buildTemperatureSets();
            buildDeltaTemperatureSets();
            buildActionSets();
            _ready = true;
            buildRelationNow(true);
        }
        #endregion

        #region Event handlers

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

        protected void buildTemperatureSets()
        {
            temperature = new ContinuousDimension("t", "Temperature detected by sensor", "°C", -30, +40);

            lowTemperature = new RightQuadraticSet(temperature, "Low temperature", 10, 15, 20);
            highTemperature = new LeftQuadraticSet(temperature, "High temperature", 20, 25, 30);
            correctTemperature = new BellSet(temperature, "Correct temperature", 20, 5, 10);

            #region Show it graphically
            RelationImage imgLowTemp = new RelationImage(lowTemperature);
            RelationImage imgHighTemp = new RelationImage(highTemperature);
            RelationImage imgCorrectTemp = new RelationImage(correctTemperature);

            Bitmap bmpLowTemp = new Bitmap(pictureBoxLowTemp.Width, pictureBoxLowTemp.Height);
            Bitmap bmpHighTemp = new Bitmap(pictureBoxHighTemp.Width, pictureBoxHighTemp.Height);
            Bitmap bmpCorrectTemp = new Bitmap(pictureBoxCorrectTemp.Width, pictureBoxCorrectTemp.Height);

            imgLowTemp.DrawImage(Graphics.FromImage(bmpLowTemp));
            imgHighTemp.DrawImage(Graphics.FromImage(bmpHighTemp));
            imgCorrectTemp.DrawImage(Graphics.FromImage(bmpCorrectTemp));

            pictureBoxLowTemp.Image = bmpLowTemp;
            pictureBoxHighTemp.Image = bmpHighTemp;
            pictureBoxCorrectTemp.Image = bmpCorrectTemp;
            #endregion

        }

        protected void buildDeltaTemperatureSets()
        {
            deltaTemperature = new ContinuousDimension("∆t", "Change of temperature detected by temperature sensor in time period", "°C/min", -5, +5);

            fallingTemperature = new RightQuadraticSet(deltaTemperature, "Falling temperature", -3, -1, 0);
            risingTemperature = new LeftQuadraticSet(deltaTemperature, "Rising temperature", 0, 1, 3);
            constantTemperature = new BellSet(deltaTemperature, "Constant temperature", 0, 1, 3);

            #region Show it graphically
            RelationImage imgFallingTemp = new RelationImage(fallingTemperature);
            RelationImage imgRisingTemp = new RelationImage(risingTemperature);
            RelationImage imgConstantTemp = new RelationImage(constantTemperature);

            Bitmap bmpFallingTemp = new Bitmap(pictureBoxFallingTemp.Width, pictureBoxFallingTemp.Height);
            Bitmap bmpRisingTemp = new Bitmap(pictureBoxRisingTemp.Width, pictureBoxRisingTemp.Height);
            Bitmap bmpConstantTemp = new Bitmap(pictureBoxConstantTemp.Width, pictureBoxConstantTemp.Height);

            imgFallingTemp.DrawImage(Graphics.FromImage(bmpFallingTemp));
            imgRisingTemp.DrawImage(Graphics.FromImage(bmpRisingTemp));
            imgConstantTemp.DrawImage(Graphics.FromImage(bmpConstantTemp));

            pictureBoxFallingTemp.Image = bmpFallingTemp;
            pictureBoxRisingTemp.Image = bmpRisingTemp;
            pictureBoxConstantTemp.Image = bmpConstantTemp;
            #endregion

        }



        protected void buildActionSets()
        {
            action = new ContinuousDimension("Action", "-10 Cool ... 0 Do nothing ... +10 heat", "", -10, +10);

            heat = new SingletonSet(action, "Heat", 10);
            cool = new SingletonSet(action, "Cool", -10);
            doNothing = new SingletonSet(action, "Do nothing", 0);

            #region Show it graphically
            RelationImage imgHeat = new RelationImage(heat);
            RelationImage imgCool = new RelationImage(cool);
            RelationImage imgDoNothing = new RelationImage(doNothing);

            Bitmap bmpHeat = new Bitmap(pictureBoxHeat.Width, pictureBoxHeat.Height);
            Bitmap bmpCool = new Bitmap(pictureBoxCool.Width, pictureBoxCool.Height);
            Bitmap bmpDoNothing = new Bitmap(pictureBoxDoNothing.Width, pictureBoxDoNothing.Height);

            imgHeat.DrawImage(Graphics.FromImage(bmpHeat));
            imgCool.DrawImage(Graphics.FromImage(bmpCool));
            imgDoNothing.DrawImage(Graphics.FromImage(bmpDoNothing));

            pictureBoxHeat.Image = bmpHeat;
            pictureBoxCool.Image = bmpCool;
            pictureBoxDoNothing.Image = bmpDoNothing;
            #endregion
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

            decimal inputTemperature = txtTemp.Value;
            decimal inputDeltaTemperature = txtDeltaTemp.Value;

            #region Realtime expression evaluation by means of C# parser
            string strExpression = txtExpression.Text;
            prependFullName(ref strExpression, "lowTemperature");
            prependFullName(ref strExpression, "highTemperature");
            prependFullName(ref strExpression, "correctTemperature");
            prependFullName(ref strExpression, "risingTemperature");
            prependFullName(ref strExpression, "fallingTemperature");
            prependFullName(ref strExpression, "constantTemperature");
            prependFullName(ref strExpression, "heat");
            prependFullName(ref strExpression, "cool");
            prependFullName(ref strExpression, "doNothing");

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
            DefuzzificationFactory.DefuzzificationMethod method = DefuzzificationFactory.DefuzzificationMethod.CenterOfMaximum;
            _defuzzification = DefuzzificationFactory.GetDefuzzification(
                _relation,
                new Dictionary<IDimension, decimal> {
                        { temperature, inputTemperature },
                        { deltaTemperature, inputDeltaTemperature }
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


            #region restoring TreeView selection
            if ((!_expressionChanged || initial) && selectedNodePath.Count() > 0 && selectedNodePath[selectedNodePath.Count() - 1] < treeViewRelation.Nodes.Count)
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
            this.Hide();
            _parentForm.Show();
            this.Close();
        }

        private void TemperatureExample_FormClosed(object sender, FormClosedEventArgs e)
        {
            //closed by the switch button?
            if (! (!this.Visible && _parentForm.Visible))
                //no => Close the whole application, including the parent form
                _parentForm.Close();
        }
        #endregion

   }
}
