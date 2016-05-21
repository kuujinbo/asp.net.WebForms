/* ###########################################################################
 * inline code parser for .aspx pages
 * http://weblogs.asp.net/infinitiesloop/archive/2006/08/09/The-CodeExpressionBuilder.aspx
 * ###########################################################################
 */
// ###########################################################################
// USAGE:
// ###########################################################################
// <asp:CheckBox id='chk1' runat='server' Text='<%$ Code: DateTime.Now %>' />
// ###########################################################################
 
using System;
using System.CodeDom;
using System.Web.Compilation;
using System.Web.UI;
/*
 */
namespace kuujinbo.asp.net.WebForms {
  [ExpressionPrefix("Code")]  
  public class CodeExpressionBuilder : ExpressionBuilder {
// ============================================================================  
    public override CodeExpression GetCodeExpression(
        BoundPropertyEntry entry, 
        object parsedData, 
        ExpressionBuilderContext context
    ) 
    {
      return new CodeSnippetExpression(entry.Expression);
    } 
// ============================================================================     
  }
}