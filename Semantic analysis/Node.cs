/*
 * A01373670 - Rodrigo Garcia Lopez
 * A01372074 - Jorge Alexis Rubio Sumano
 * A01371084 - Valentin Ochoa Lopez
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Chimera
{

    public class Node : IEnumerable<Node>
    {

        IList<Node> children = new List<Node>();

        public Int32 Count()
        {
            return children.Count;
        }

        public Node this[int index]
        {
            get
            {
                return children[index];
            }
        }

        public Token AnchorToken { get; set; }

        public void Add(Node node)
        {
            children.Add(node);
        }

        public dynamic ExtractValue()
        {
            if (AnchorToken.Category == TokenCategory.TRUE )
                return true;
            if (AnchorToken.Category == TokenCategory.FALSE)
                return false;
            if (AnchorToken.Category == TokenCategory.INT_LITERAL)
                return Convert.ToInt32(AnchorToken.Lexeme);
            if (AnchorToken.Category == TokenCategory.STR_LITERAL)
                return AnchorToken.Lexeme;
            throw new FieldAccessException("Expecting one of the followings tokens categories: TRUE, FALSE, INT_LITERAL, STR_LITERAL");
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator
                System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", GetType().Name, AnchorToken);
        }

        public string ToStringTree()
        {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }

        static void TreeTraversal(Node node, string indent, StringBuilder sb)
        {
            sb.Append(indent);
            sb.Append(node);
            sb.Append('\n');
            foreach (var child in node.children)
            {
                TreeTraversal(child, indent + "  ", sb);
            }
        }
    }
}
