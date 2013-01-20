/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/
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
