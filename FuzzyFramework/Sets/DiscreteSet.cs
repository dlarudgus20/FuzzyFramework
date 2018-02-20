using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FuzzyFramework.Dimensions;
using FuzzyFramework.Intervals;
using FuzzyFramework.Members;

namespace FuzzyFramework.Sets
{
    /// <summary>
    /// A Discrete fuzzy set. Another alternative is a continuous fuzzy set. Considering standard (crisp) discrete and continuous sets, discere sets are actually enumerations, whereas continuos sets are intervals.
    /// Things works similar in the fuzzy set theory, the only difference is the fuzzy membership instead of boolean membership. As the result, discrete fuzzy set is an enumeration of pairs {Element; Membership}. Continous set has to be described by a function Element -> Membership.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DiscreteSet : FuzzySet
    {
        #region private
        /// <summary>
        /// The enumeration of members with the membership greater than 0
        /// </summary>
        Dictionary<IMember, double> _members = new Dictionary<IMember, double>();
        
        /// <summary>
        /// How many members do we want to display on a graph
        /// </summary>
        private static readonly uint MaxMembersToDisplay = 10;

        #endregion

        #region public methods

        public DiscreteSet(IDiscreteDimension dimension)
            : base(dimension)
        {
            _intervals = new IntervalSet(dimension);
        }

        public DiscreteSet(IDiscreteDimension dimension, IntervalSet intervals)
            : base(dimension)
        {
            _intervals = intervals;
        }



        public DiscreteSet(IDiscreteDimension dimension, string caption)
            : base(dimension, caption)
        {
            _intervals = new IntervalSet(dimension);
        }


        /// <summary>
        /// Adds another member to this set.
        /// </summary>
        /// <param name="memberToAdd">The member to add</param>
        /// <param name="membershipDegree">Membership degree of the member. To create a crisp discrete set, this parameter would have to be always 1. Note: Do not add members with zero membership. Just avoid calling this method for the specific member at all.</param>
        public void AddMember(IMember memberToAdd, double membershipDegree)
        {
            if(memberToAdd == null)
                throw new NullReferenceException(String.Format("Parameter memberToAdd not specified in method AddMember in fuzzy set \"{0}\".", this.Caption));
            
            if (membershipDegree < 0 || membershipDegree > 1)
                throw new MembershipOutOfRangeException( String.Format("Membership degree {0} for element \"{1}\" in fuzzy set \"{2}\" does not belong to the range of <0,1>.", membershipDegree, memberToAdd.Caption, this.Caption));
            
            
            _members.Add(memberToAdd, membershipDegree);

            //Finds position of the specified member in the dictionary collection
            //int position = _members.Keys.ToList<IMember>().FindLastIndex( delegate(IMember m) { return (m == memberToAdd); } );


            //...and also into the internal implemenation
            _intervals.AddInterval(new Interval(_intervals, memberToAdd.ToDecimal, membershipDegree));

            object o = new object();
            lock (o)
            {

                if (memberToAdd.ToDecimal >= ((IDiscreteDimension)_dimension).MemberCount)
                    ((IDiscreteDimension)_dimension).MemberCount = (uint) memberToAdd.ToDecimal + 1;
            }

            if (_dimension.SignificantValues.Length < MaxMembersToDisplay)
            {
                List<decimal> significantValues = _dimension.SignificantValues.ToList<decimal>();
                significantValues.Add(memberToAdd.ToDecimal);
                _dimension.SignificantValues = significantValues.ToArray();
            }
            
            

        }
        #endregion

        #region public properties
        /// <summary>
        /// Returns all members within this discrete fuzzy set. I.e. members where the membership > 0.
        /// </summary>
        public ReadOnlyCollection<IMember> Members
        {
            get
            {
                return new ReadOnlyCollection<IMember>(_members.Keys.ToArray<IMember>().ToList<IMember>());
            }
        }
        
        /// <summary>
        /// Return member based on its decimal representation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IMember GetMember(decimal value)
        {
            IMember result = Members.First(t => t.ToDecimal == value);
            if (result == null)
                throw new ArgumentException(String.Format("Element where AsDecimal = {0} not found in set {1}.", value, this.Caption));
            return result;
        }

        
        /// <summary>
        /// Returns all members which comply with the specified condition.
        /// </summary>
        /// <param name="membershipCondition">Expression to evaluate the membership. To return members all members where membership is more than a half, use "t=>t.Value > 0.5"</param>
        /// <returns>Members of this set that comply to the membership condition</returns>
        public ReadOnlyCollection<IMember> GetMembers(Expression<Func<KeyValuePair<IMember,double>,bool>> membershipCondition)
        {
            IQueryable<KeyValuePair<IMember, double>> filteredMembers = _members.AsQueryable<KeyValuePair<IMember, double>>().Where(membershipCondition);

            IQueryable<IMember> filteredMembersWithoutMembership = filteredMembers.Select<KeyValuePair<IMember, double>, IMember>(t => t.Key);

            IMember[] arrMembers = filteredMembersWithoutMembership.ToArray<IMember>();

            return new ReadOnlyCollection<IMember>(arrMembers);
                
        }

        /// <summary>
        /// Returns all members which belong to the membership specified by the lowest and highest membership degree.
        /// </summary>
        /// <param name="minimumMembership">Returns only members whom membership is equal or higher than the specified degree.</param>
        /// <param name="maximumMembership">Returns only members whom membership is equal or lower than the specified degree.</param>
        /// <returns></returns>
        public ReadOnlyCollection<IMember> GetMembers(double minimumMembership, double maximumMembership)
        {
            if (minimumMembership > 1 || minimumMembership < 0 || maximumMembership < minimumMembership)
                throw new ArgumentOutOfRangeException("minimumMembership", "Membership degree must be between <0,1>. Maximum membership must be higher than mimimumMembership.");

            if (maximumMembership > 1 || maximumMembership < 0)
                throw new ArgumentOutOfRangeException("maximumMembership", "Membership degree must be between <0,1>. Maximum membership must be higher than mimimumMembership.");


            return GetMembers(t => t.Value >= minimumMembership && t.Value <= maximumMembership);
        }

        /// <summary>
        /// Returns all members whom membership is at least as strong as the specified degree.
        /// </summary>
        /// <param name="minimumMembership">Minimum membership to consider an element as the member of this set.</param>
        /// <returns>Returns only members whom membership is equal or higher than the specified degree.</returns>
        public ReadOnlyCollection<IMember> GetMembers(double minimumMembership)
        {
            return GetMembers(minimumMembership, 1);
        }

        /*
        /// <summary>
        /// Returns membership degree for the specified element. 
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Membership degree of the element, or zero if the element is not a member at all.</returns>
        public override double IsMember(IMember element)
        {
            if(element == null)
                throw new NullReferenceException(String.Format("Parameter element not specified in method IsMember in fuzzy set \"{0}\".", this.Caption));

            
            if (! _members.ContainsKey(element))
            {
                return 0;
            }

            return _members[element];
        }
        */

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            uint c = 0;
            foreach(KeyValuePair<IMember, double> m in _members)
            {
                if (c > 0) sb.Append(",");
                sb.Append( String.Format( "({0},{1:F5})", m.Key.Caption, m.Value ) );
                if (++c > 20) break;
            }

            if (this._members.Count() > 20)
                sb.Append("...");

            return sb.ToString();
        }



        #endregion

    }
}
