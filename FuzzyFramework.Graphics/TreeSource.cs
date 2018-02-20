using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FuzzyFramework.Defuzzification;
using FuzzyFramework.Dimensions;
using System.Drawing;
using FuzzyFramework.Sets;
using System.Collections.ObjectModel;
using FuzzyFramework.Intervals;

namespace FuzzyFramework.Graphics
{
    /// <summary>
    /// Provides source for a TreeView component to represent a fuzzy relation in a transparent, hierarchical way.
    /// </summary>
    public class TreeSource
    {
        private FuzzyRelation _relation;
        private Defuzzification.Defuzzification _deffuzification = null;
        private Dictionary<IDimension, System.Decimal> _inputs;
        private bool _supportOnly;
        private bool _drawImageOnNodeSelect = true;

        

        IDimension _variableDimension;
        Label _label;
        PictureBox _pictureBox;

        public static readonly Color GraphBackgroundColor = Color.White;
        public static readonly Color UnspecifiedDimensionFontColor = Color.Blue;
        public static readonly Color SpecifiedDimensionFontColor = Color.Black;
        public static readonly Color VariableDimensionFontColor = Color.Orange;
        public static readonly Color MainNodeFontColor = Color.Green;
        public static readonly Color OperatorFontColor = Color.Red;


        public static readonly bool GraphSupportOnly = true;
        
        /// <summary>
        /// Creates new instance of the TreeSource for the specifed relation
        /// </summary>
        /// <param name="relation">Fuzzy relation to represent as a hierarchical list</param>
        /// <param name="inputs">Set of specified values for particular dimensions.</param>
        /// <param name="variableDimension">Dimension which should be put on the x-axis. Null if we don't want to display graphs, or if we only want to display graphs for single-diemnsional sets.</param>
        public TreeSource(FuzzyRelation relation, Dictionary<IDimension, System.Decimal> inputs, IDimension variableDimension)
        {
            _relation = relation;
            _inputs = inputs;
            _variableDimension = variableDimension;
            treeView_AfterSelect_EventHandler = new TreeViewEventHandler(treeView_AfterSelect);
        }

        /// <summary>
        /// Creates new instance of the TreeSource for the specifed relation
        /// </summary>
        /// <param name="deffuzification">Relation wrapped in a deffuzification. In this case, the hierarchy will also contain information about applied deffuzification.</param>
        public TreeSource(FuzzyFramework.Defuzzification.Defuzzification deffuzification)
        {
            _relation = deffuzification.Relation;
            _deffuzification = deffuzification;
            _inputs = deffuzification.Inputs;
            _variableDimension = deffuzification.OutputDimension;
            treeView_AfterSelect_EventHandler = new TreeViewEventHandler(treeView_AfterSelect);
        }
        
        /// <summary>
        /// Fills in the specified System.Windows.Forms.TreeView component with content representing the fuzzy relation
        /// </summary>
        /// <param name="treeView">Treeview component to display the relation's hierararchy</param>
        /// <param name="pictureBox">PictureBox component to display graphs for particular nodes, where possible. Null if graphs not required.</param>
        /// <param name="label">Label to display caption for the picture. Null if graphs not required.</param>
        public void BuildTree ( TreeView treeView, PictureBox pictureBox, Label label)
        {
            _pictureBox = pictureBox;
            _label = label;
            
            ImageList imageList = new ImageList();
            imageList.Images.Add("dimensions", Resources.dimensions);
            imageList.Images.Add("function", Resources.Function1);
            imageList.Images.Add("subrelations", Resources.subrelations);
            imageList.Images.Add("fuzzySet", Resources.fuzzySet);
            imageList.Images.Add("nodeFuzzyRelation", Resources.nodeFuzzyRelation);
            imageList.Images.Add("dimensionType", Resources.dimensionType);
            imageList.Images.Add("dimensionDiscreteKnown", Resources.dimensionDiscrete);
            imageList.Images.Add("dimensionDiscreteUnknown", Resources.dimensionDiscreteUnknown);
            imageList.Images.Add("dimensionContinuousKnown", Resources.dimensionContinuous);
            imageList.Images.Add("dimensionContinuousUnknown", Resources.dimensionContinuousUnknown);
            imageList.Images.Add("defuzzification", Resources.defuzzification);
            imageList.Images.Add("defuzzificationOutput", Resources.defuzzificationOutput);
            imageList.Images.Add("spacer", Resources.spacer);    
 


            treeView.ImageList = imageList;
            pictureBox.BackColor = GraphBackgroundColor;
            pictureBox.Image = null;
            label.Text = "";
            
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            if (_deffuzification == null)
            {
                buildSubTree(_relation, treeView.Nodes, pictureBox, label);
            }
            else
            {
                TreeNode deffuz = new TreeNode(_deffuzification.GetType().Name);
                deffuz.ImageKey = "defuzzification";
                deffuz.SelectedImageKey = "defuzzification";
                deffuz.Tag = _deffuzification;
                treeView.Nodes.Add(deffuz);
                TreeNode deffuzOutput = new TreeNode(
                    String.Format("{0}={1:F5} {2}",
                        _deffuzification.OutputDimension.Name,
                        _deffuzification.CrispValue,
                        _deffuzification.OutputDimension is IContinuousDimension ? ((IContinuousDimension)_deffuzification.OutputDimension).Unit : ""
                    )
                );
                deffuzOutput.ImageKey = "defuzzificationOutput";
                deffuzOutput.SelectedImageKey = "defuzzificationOutput";
                deffuz.Nodes.Add(deffuzOutput);
                buildSubTree(_relation, deffuz.Nodes, pictureBox, label);
            }

            treeView.EndUpdate();

            treeView.AfterSelect -= treeView_AfterSelect_EventHandler;
            if (_pictureBox != null && _label != null)
                treeView.AfterSelect += treeView_AfterSelect_EventHandler;
        }


        /// <summary>
        /// Draws detail image and prints label for the specified treenode
        /// </summary>
        /// <param name="treeNode"></param>
        public void DrawDetailImage(TreeNode treeNode)
        {
            treeView_AfterSelect(null, new TreeViewEventArgs(treeNode));
        }

        /// <summary>
        /// True if image should be drawn and label should be printed on tree node select.
        /// Anyway, image and label can be always generated explicitly by means of method DrawDetailImage.
        /// </summary>
        public bool DrawImageOnNodeSelect
        {
            get
            {
                return _drawImageOnNodeSelect;
            }

            set
            {
                _drawImageOnNodeSelect = value;
            }
        }


        protected TreeViewEventHandler treeView_AfterSelect_EventHandler;
        
        void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_pictureBox != null && _label != null)
            {
                //Find nearest relation upper in the hierarchy
                TreeNode tn = e.Node;
                while (tn.Tag == null && !(tn.Tag is FuzzyRelation) && !(tn.Tag is Defuzzification.Defuzzification))
                {
                    if (tn.Parent == null)
                    {   //We are at the top
                        _label.Text = "";
                        _pictureBox.Image = null;
                        tn = null;
                        break;
                    }
                    tn = tn.Parent;
                }

                if (tn != null)
                {
                    if (tn.Tag is Defuzzification.Defuzzification)
                    {
                        Defuzzification.Defuzzification deffuz = (Defuzzification.Defuzzification) tn.Tag;

                        if (allInputDimensionsAvailable(deffuz.Relation))
                        {
                            Dictionary<IDimension, System.Decimal> inputsCopy = new Dictionary<IDimension, System.Decimal>(_inputs);
                            if (!inputsCopy.ContainsKey(deffuz.OutputDimension))
                                inputsCopy.Add(deffuz.OutputDimension, deffuz.CrispValue);
                            else
                                inputsCopy[deffuz.OutputDimension] = deffuz.CrispValue;

                            RelationImage img = new RelationImage(deffuz.Relation, inputsCopy, deffuz.OutputDimension);
                            img.SupportOnly = _supportOnly;
                            Bitmap bmp = new Bitmap(_pictureBox.Width, _pictureBox.Height);
                            img.DrawImage(System.Drawing.Graphics.FromImage(bmp));
                            _pictureBox.Image = bmp;

                            _label.Text = String.Format("{0} deffuzification yields {1}={2:F5} {3}",
                                deffuz.GetType().Name,
                                deffuz.OutputDimension.Name,
                                deffuz.CrispValue,
                                deffuz.OutputDimension is IContinuousDimension ? ((IContinuousDimension)deffuz.OutputDimension).Unit : ""
                             );
                        }else
                            reportTooManyDimensions();
                    }
                    else
                    {
                        FuzzyRelation relation = (FuzzyRelation)tn.Tag;
                        if (allInputDimensionsAvailable(relation))
                        {
                            RelationImage img = null;
                            if ( relation.Dimensions.Count() == 1)
                                img = new RelationImage(relation, _inputs, relation.Dimensions[0]);
                            else if (_variableDimension != null)
                                img = new RelationImage(relation, _inputs, _variableDimension);

                            if (img == null)
                            {
                                _label.Text = "";
                                _pictureBox.Image = null;
                            }
                            else
                            {
                                string lblText = "";
                                img.SupportOnly = _supportOnly;
                                Bitmap bmp = new Bitmap(_pictureBox.Width, _pictureBox.Height);
                                img.DrawImage(System.Drawing.Graphics.FromImage(bmp));
                                _pictureBox.Image = bmp;
                                IDimension realVariableDimension;
                                if (relation.Dimensions.Count() == 1)
                                {
                                    lblText = String.Format("Fuzzy set for dimension {0}.", relation.Dimensions[0].Name);
                                    realVariableDimension = relation.Dimensions[0];
                                }
                                else
                                {
                                    StringBuilder sb = new StringBuilder();
                                    foreach(IDimension dim in relation.Dimensions)
                                    {
                                        if (sb.Length != 0)
                                            sb.Append(" x ");
                                        sb.Append(dim.Name);
                                     }
                                    lblText = String.Format("Fuzzy relation for dimensions ({0}) where {1} is variable.", sb.ToString(), _variableDimension.Name);
                                    realVariableDimension = _variableDimension;
                                }

                                if (_inputs.ContainsKey(realVariableDimension))
                                {
                                    string value;
                                    if (realVariableDimension is IContinuousDimension)
                                    {
                                        IContinuousDimension dim = (IContinuousDimension) realVariableDimension;
                                        value = _inputs[realVariableDimension].ToString("F5");
                                        if (! String.IsNullOrEmpty(dim.Unit) )
                                            value += " " + dim.Unit;
                                    }
                                    else
                                    {
                                        IDiscreteDimension dim = (IDiscreteDimension) realVariableDimension;
                                        if (dim.DefaultSet != null)
                                            value = dim.DefaultSet.GetMember(_inputs[realVariableDimension]).Caption;
                                        else
                                            value = "#" + _inputs[realVariableDimension].ToString("F0");
                                    }

                                    lblText += String.Format("\r\nµ{1}(x)={0:F2} for x={2}.",
                                        relation.IsMember(_inputs),
                                        realVariableDimension.Name,
                                        value
                                     );
                                        
                                }
                                _label.Text = lblText;
                            }

                        }

                    }
                }
            }
        }

        protected bool allInputDimensionsAvailable(FuzzyRelation relation)
        {
            if (relation.Dimensions.Count() == 1)
                return true;

            if (_variableDimension == null)
                return false;

            foreach (IDimension dimension in relation.Dimensions)
            {
                if (!_inputs.ContainsKey( dimension) && dimension != _variableDimension)
                    return false;
             }

            return true;
        }

        protected void reportTooManyDimensions()
        {
            _pictureBox.Image = Resources.ToManyDimensions;
            _label.Text = "";
        }

        /// <summary>
        /// Method to be invoked recursively to build the whole tree
        /// </summary>
        /// <param name="nodeCollection"></param>
        /// <param name="pictureBox"></param>
        /// <param name="label"></param>
        protected void buildSubTree(FuzzyRelation subrelation, TreeNodeCollection nodeCollection, PictureBox pictureBox, Label label)
        {
            TreeNode tnThis;

 
            if (subrelation is FuzzySet)
            {
                FuzzySet fs = (FuzzySet)subrelation;
                tnThis = new TreeNode();
                if (!String.IsNullOrEmpty(fs.Caption))
                {
                    tnThis.Text = fs.Caption;
                }
                else
                {
                    tnThis.Text = "Fuzzy Set";
                }
                tnThis.ImageKey = "fuzzySet";
                tnThis.SelectedImageKey = "fuzzySet";

                TreeNode tnDimType = new TreeNode("Type: " + (fs.Dimensions[0] is IContinuousDimension ? "Continuous" : "Discrete"));
                tnDimType.ImageKey = "dimensionType";
                tnDimType.SelectedImageKey = "dimensionType";
                tnThis.Nodes.Add(tnDimType);
            }
            else
            {
                NodeFuzzyRelation nfr = (NodeFuzzyRelation)subrelation;
                tnThis = new TreeNode("Multidimensional Relation");
                tnThis.ImageKey = "nodeFuzzyRelation";
                tnThis.SelectedImageKey = "nodeFuzzyRelation";

                TreeNode tnSubrelations = new TreeNode(nfr.Operator.Caption);
                tnSubrelations.ImageKey = "subrelations";
                tnSubrelations.SelectedImageKey = "subrelations";
                tnSubrelations.ForeColor = OperatorFontColor;
                tnThis.Nodes.Add(tnSubrelations);

                //Find all operands. Several commutative operands of same type from different nested levels will be displayed together
                List<FuzzyRelation> nestedSubrelations = new List<FuzzyRelation>();
                findNestedOperands(nfr, nestedSubrelations);

                foreach(FuzzyRelation nestedSubrelation in nestedSubrelations)
                {
                    buildSubTree(nestedSubrelation, tnSubrelations.Nodes, pictureBox, label);
                }
            }

            #region Dimensions

            TreeNode tnDimensions = new TreeNode("Dimension" + ((subrelation.Dimensions.Count() > 1) ? "s" : "") );
            tnDimensions.ImageKey = "dimensions";
            tnDimensions.SelectedImageKey = "dimensions";
            tnThis.Nodes.Add(tnDimensions);

            foreach(IDimension dimension in subrelation.Dimensions)
            {
                bool blnKnown = _inputs.ContainsKey(dimension);
                bool blnContinuous = dimension is IContinuousDimension;
                Color fontColor;

                string strDimCaption = String.IsNullOrEmpty(dimension.Name) ? "Dimension" : dimension.Name;

                if (blnKnown)
                {
                    if (blnContinuous)
                    {
                        strDimCaption += String.Format("={0:F5} {1}", _inputs[dimension], ((IContinuousDimension)dimension).Unit);
                    }
                    else
                    {
                        IDiscreteDimension discreteDim = (IDiscreteDimension)dimension;
                        if (discreteDim.DefaultSet != null)
                            strDimCaption += "=" + discreteDim.DefaultSet.GetMember(_inputs[dimension]).Caption;
                        else
                            strDimCaption += String.Format("=#{0:F0}", _inputs[dimension]);
                    }
                    fontColor = SpecifiedDimensionFontColor;
                }
                else
                    fontColor = UnspecifiedDimensionFontColor;

                if (dimension == _variableDimension)
                    fontColor = VariableDimensionFontColor;

                string imageKey = String.Format("dimension{0}{1}", blnContinuous ? "Continuous" : "Discrete", blnKnown ? "Known" : "Unknown");

                TreeNode tnDimension = new TreeNode(strDimCaption);
                tnDimension.ImageKey = imageKey;
                tnDimension.SelectedImageKey = imageKey;
                tnDimension.ForeColor = fontColor;
                addToolTip(tnDimension, dimension.Description);

                tnDimensions.Nodes.Add(tnDimension);
            }
            #endregion

            #region Function
            if (allInputDimensionsAvailable(subrelation))
            {
                IDimension realVariableDimension;
                if (subrelation.Dimensions.Count() == 1)
                    realVariableDimension = subrelation.Dimensions[0];
                else
                    realVariableDimension = _variableDimension;

                Dictionary<IDimension, decimal> copyInputs = new Dictionary<IDimension,decimal>( _inputs );

                foreach (KeyValuePair<IDimension, decimal> item in _inputs)
                    if (!subrelation.Dimensions.Contains(item.Key))
                        copyInputs.Remove(item.Key);

                if (copyInputs.ContainsKey(realVariableDimension))
                    copyInputs.Remove(realVariableDimension);

                if (subrelation.Dimensions.Count() > copyInputs.Count())
                {

                    IntervalSet intervals = subrelation.GetFunction(copyInputs);

                    string strIntervals = intervals.ToString();

                    string[] arrLines = strIntervals.Split(new char[] { '\n' });

                    TreeNode tnFunction = new TreeNode("Function");
                    tnFunction.ImageKey = "function";
                    tnFunction.SelectedImageKey = "function";
                    foreach (string line in arrLines)
                    {
                        if (!String.IsNullOrWhiteSpace(line))
                        {
                            TreeNode tnLine = new TreeNode(line);
                            tnLine.ImageKey = "spacer";
                            tnLine.SelectedImageKey = "spacer";
                            tnFunction.Nodes.Add(tnLine);
                        }
                    }

                    tnThis.Nodes.Add(tnFunction);
                }
            }
            
            #endregion

            tnThis.ForeColor = MainNodeFontColor;
            tnThis.Tag = subrelation;
            nodeCollection.Add(tnThis);
        }

        protected void addToolTip(TreeNode node, string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                node.ToolTipText = text;
            }
        }

        /// <summary>
        /// Several commutative operands of same type from different nested levels will be joined in a single list
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="foundNestedSubrelations"></param>
        protected void findNestedOperands(NodeFuzzyRelation relation, List<FuzzyRelation> foundNestedSubrelations)
        {
            if (relation.Subrelation1 is NodeFuzzyRelation && ((NodeFuzzyRelation)relation.Subrelation1).Operator.GetType() == relation.Operator.GetType())
            {
                   findNestedOperands((NodeFuzzyRelation)relation.Subrelation1, foundNestedSubrelations);
            }
            else
            {
                foundNestedSubrelations.Add(relation.Subrelation1);
            }

            if (relation.Subrelation2 != null)
            {
                if (relation.Subrelation2 is NodeFuzzyRelation && ((NodeFuzzyRelation)relation.Subrelation2).Operator.GetType() == relation.Operator.GetType())
                {
                    findNestedOperands((NodeFuzzyRelation)relation.Subrelation2, foundNestedSubrelations);
                }
                else
                {
                    foundNestedSubrelations.Add(relation.Subrelation2);
                }
            }
        }


        /// <summary>
        /// Relation to display
        /// </summary>
        public FuzzyRelation Relation
        {
            get
            {
                return _relation;
            }
        }

        /// <summary>
        /// Deffuzification applied on the fuzzy relation. Null if no deffuzification specified.
        /// </summary>
        public Defuzzification.Defuzzification Deffuzification
        {
            get
            {
                return _deffuzification;
            }
        }

        /// <summary>
        /// If True, graphs will be displayed for the area around support on x-axis.
        /// If False, x-axis will contain whole universe.
        /// </summary>
        public bool SupportOnly
        {
            get { return _supportOnly; }
            set { _supportOnly = value; }
        }
    }
}
