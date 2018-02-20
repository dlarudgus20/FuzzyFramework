using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FuzzyFramework.Deffuzification;
using FuzzyFramework.Dimensions;
using System.Drawing;
using FuzzyFramework.Sets;
using System.Collections.ObjectModel;

namespace FuzzyFramework.Graphics
{
    /// <summary>
    /// Provides source for a TreeView component to represent a fuzzy relation in a transparent, hierarchical way.
    /// </summary>
    public class TreeSource
    {
        private FuzzyRelation _relation;
        private Deffuzification.Defuzzification _deffuzification = null;
        private Dictionary<IDimension, System.Decimal> _inputs;
        IDimension _variableDimension;

        public static readonly Color GraphBackgroundColor = Color.White;
        public static readonly Color UnspecifiedDimensionFontColor = Color.Blue;
        public static readonly Color SpecifiedDimensionFontColor = Color.Black;
        public static readonly Color VariableDimensionFontColor = Color.Orange;
        public static readonly Color UnnamedNodeFontColor = Color.Green;
        public static readonly Color OperatorFontColor = Color.Red;


        public static readonly bool GraphSupportOnly = true;
        
        /// <summary>
        /// Creates new instance of the TreeSource for the specifed relation
        /// </summary>
        /// <param name="relation">Fuzzy relation to represent as a hierarchical list</param>
        /// <param name="inputs">Set of specified values for particular dimensions.</param>
        /// <param name="variableDimension">Dimension which should be put on the x-axis</param>
        public TreeSource(FuzzyRelation relation, Dictionary<IDimension, System.Decimal> inputs, IDimension variableDimension)
        {
            _relation = relation;
            _inputs = inputs;
            _variableDimension = variableDimension;
        }

        /// <summary>
        /// Creates new instance of the TreeSource for the specifed relation
        /// </summary>
        /// <param name="deffuzification">Relation wrapped in a deffuzification. In this case, the hierarchy will also contain information about applied deffuzification.</param>
        public TreeSource(Defuzzification deffuzification)
        {
            _relation = deffuzification.Relation;
            _deffuzification = deffuzification;
            _inputs = deffuzification.Inputs;
            _variableDimension = deffuzification.OutputDimension;
        }
        
        /// <summary>
        /// Fills in the specified System.Windows.Forms.TreeView component with content representing the fuzzy relation
        /// </summary>
        /// <param name="treeView">Treeview component to display the relation's hierararchy</param>
        /// <param name="pictureBox">PictureBox component to display grapfs for particular nodes, where possible</param>
        /// <param name="label">Label to display caption for the picture</param>
        public void BuildTree ( TreeView treeView, PictureBox pictureBox, Label label)
        {
            ImageList imageList = new ImageList();
            imageList.Images.Add("dimensions", Resources.Graph);
            imageList.Images.Add("function", Resources.Function);
            imageList.Images.Add("subrelations", Resources.Function);
            imageList.Images.Add("fuzzySet", Resources.Function);
            imageList.Images.Add("nodeFuzzyRelation", Resources.Function);
            imageList.Images.Add("dimensionType", Resources.Function);
            imageList.Images.Add("dimensionDiscreteKnown", Resources.Function);
            imageList.Images.Add("dimensionDiscreteUnknown", Resources.Function);
            imageList.Images.Add("dimensionContinuousKnown", Resources.Function);
            imageList.Images.Add("dimensionContinuousUnknown", Resources.Function);

 
            RelationImage img = new RelationImage(_relation, _inputs, _variableDimension);
            img.SupportOnly = true;

            Bitmap bmp = new Bitmap(300, 200);
            imageList.Images.Add(bmp);

            treeView.ImageList = imageList;
            pictureBox.BackColor = GraphBackgroundColor;
            pictureBox.Image = null;
            label.Text = "";
            
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            buildSubTree(_relation, treeView.Nodes, pictureBox, label);

            treeView.EndUpdate();
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
                    tnThis.ForeColor = UnnamedNodeFontColor;
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
                tnThis.ForeColor = UnnamedNodeFontColor;
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

            #region Dimensions;

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
                            strDimCaption += "=" + discreteDim.DefaultSet.GetMember(_inputs[dimension]);
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
            
            #endregion
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
        public Deffuzification.Defuzzification Deffuzification
        {
            get
            {
                return _deffuzification;
            }
        }
    }
}
