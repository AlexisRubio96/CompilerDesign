/*
  Chimera compiler - This class performs the lexical analysis.
  Copyright @2019 by Valentín Ochoa López, Rodrigo García López & 
  Jorge Alexis Rubio  
  
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.
  
  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  
  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Chimera {

    class Scanner {

        readonly string input;

        static readonly Regex regex = new Regex(
            @"
                (?<Newline>         \n                          )
              | (?<WhiteSpace>      \s                          )
              | (?<LineComment>     //.*                        )
              | (?<BlockComment>    [/][*](.|\n)*?[*][/]        )
              | (?<Identifier>      [a-zA-Z](\w|_)*             )
              | (?<IntLiteral>      \d+                         )
              | (?<String>          [""].*?[""]{1}([""]{2})*    ) 
              | (?<Semicolon>       ;                           )
              | (?<Other>           .                           ) # Match any other character
            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );
       
        static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"program", TokenCategory.PROGRAM},
                {"end", TokenCategory.END},
            };

        static readonly IDictionary<string, TokenCategory> symbols =
            new Dictionary<string, TokenCategory>() {
                {"Semicolon", TokenCategory.SEMICOLON},
            };

        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Start() {

            var row = 1;
            var columnStart = 0;

            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {
                if (m.Groups["Newline"].Success)
                {
                    row++;
                    columnStart = m.Index + m.Length;
                }
                else if (m.Groups["WhiteSpace"].Success)
                {
                    continue;
                }
                else if (m.Groups["LineComment"].Success)
                {
                    continue;
                }
                else if (m.Groups["BlockComment"].Success)
                {
                    row += m.Value.Split('\n').Length - 1;
                }
                else if (m.Groups["Identifier"].Success)
                {
                    if (keywords.ContainsKey(m.Value))
                    {
                        // Matched string is a Chimera keyword.
                        yield return newTok(m, keywords[m.Value]);
                    }
                    else
                    {
                        // Otherwise it's just a plain identifier.
                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }
                }
                else if (m.Groups["IntLiteral"].Success)
                {
                    yield return newTok(m, TokenCategory.INT_LITERAL);
                }
                else if (m.Groups["String"].Success)
                {
                    yield return newTok(m, TokenCategory.INT_LITERAL);
                }
                else if (m.Groups["Other"].Success)
                {
                    // Found an illegal character.
                    yield return newTok(m, TokenCategory.ILLEGAL_CHAR);
                }
                else 
                {
                    // Match must be one of the symbols.
                    foreach (var name in symbols.Keys) 
                    {
                        if (m.Groups[name].Success) 
                        {
                            yield return newTok(m, symbols[name]);
                            break;
                        }
                    }
                }
            }

            yield return new Token(null, 
                                   TokenCategory.EOF, 
                                   row, 
                                   input.Length - columnStart + 1);
        }
    }
}
