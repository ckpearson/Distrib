using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib
{
    /// <summary>
    /// Exception helper class
    /// </summary>
    public static class Ex
    {
        /// <summary>
        /// Generate an argument null exception
        /// </summary>
        /// <param name="expr">Point to the offending argument</param>
        /// <returns>The argument null exception</returns>
        public static ArgumentNullException ArgNull(Expression<Func<object>> expr)
        {
            if (expr == null) throw new ArgumentNullException("Expression needed");

            try
            {
                var member = GetMemberFromExpr(expr);

                return new ArgumentNullException(member.Name, string.Format(
                    "Argument '{0}' must be supplied", member.Name));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create argument null exception", ex);
            }
        }

        

        public static ArgumentException Arg(Expression<Func<object>> expr, string message = "")
        {
            if (expr == null) throw new ArgumentNullException("Expression must be provided");

            try
            {
                var mem = GetMemberFromExpr(expr);
                return new ArgumentException(string.IsNullOrEmpty(message) ?
                    string.Format("There was a problem with argument '{0}'", mem.Name) : message,
                    mem.Name);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create argument exception", ex);
            }
        }

        #region Private Helper Methods
        private static bool IsMemberExpression(Expression<Func<object>> expr)
        {
            if (expr == null) throw new ArgumentNullException("Expression must be provided");

            try
            {
                return expr.Body is MemberExpression;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if expression is member expression", ex);
            }
        }

        private static MemberInfo GetMemberFromExpr(Expression<Func<object>> expr)
        {
            if (expr == null) throw new ArgumentNullException("Expression must be provided");

            try
            {
                if (!IsMemberExpression(expr))
                {
                    throw new ArgumentException("Expression must be a member expression");
                }

                return ((MemberExpression)expr.Body).Member;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get member from expression", ex);
            }
        } 
        #endregion
    }
}
