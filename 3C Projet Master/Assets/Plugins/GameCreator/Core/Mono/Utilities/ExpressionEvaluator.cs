namespace GameCreator.Core
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class ExpressionEvaluator
    {
        private const string ERR_PARENTHESIS = "Mis-matched parentheses in expression: {0}";
        private const string ERR_NUMLITS = "Mis-matched number of literals in math expression: {0}";

        private static readonly string[] OPERATORS = { 
            "-", 
            "+", 
            "/", 
            "*",
            "^",
        };

        private static Func<float, float, float>[] OPERATIONS = {
            (a1, a2) => a1 - a2,
            (a1, a2) => a1 + a2,
            (a1, a2) => a1 / a2,
            (a1, a2) => a1 * a2,
            (a1, a2) => Mathf.Pow(a1, a2),
        };

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static float Evaluate(string expression)
        {
            List<string> tokens = ExpressionEvaluator.GetTokens(expression);
            Stack<float> operandStack = new Stack<float>();
            Stack<string> operatorStack = new Stack<string>();
            int tokenIndex = 0;

            while (tokenIndex < tokens.Count)
            {
                string token = tokens[tokenIndex];
                if (token == "(")
                {
                    string subExpr = GetSubExpression(tokens, ref tokenIndex);
                    operandStack.Push(Evaluate(subExpr));
                    continue;
                }

                if (token == ")")
                {
                    throw new ArgumentException(string.Format(ERR_PARENTHESIS, expression));
                }

                if (Array.IndexOf(OPERATORS, token) >= 0)
                {
                    while (operatorStack.Count > 0 && 
                           Array.IndexOf(OPERATORS, token) < Array.IndexOf(OPERATORS, operatorStack.Peek()))
                    {
                        string op = operatorStack.Pop();
                        float arg2 = operandStack.Pop();
                        float arg1 = operandStack.Pop();
                        operandStack.Push(OPERATIONS[Array.IndexOf(OPERATORS, op)](arg1, arg2));
                    }

                    operatorStack.Push(token);
                }
                else
                {
                    operandStack.Push(float.Parse(token));
                }

                tokenIndex += 1;
            }

            while (operatorStack.Count > 0)
            {
                string op = operatorStack.Pop();
                float arg2 = operandStack.Pop();
                float arg1 = operandStack.Pop();

                operandStack.Push(OPERATIONS[Array.IndexOf(OPERATORS, op)](arg1, arg2));
            }

            return operandStack.Pop();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static string GetSubExpression(List<string> tokens, ref int index)
        {
            StringBuilder subExpr = new StringBuilder();
            int parenlevels = 1;
            index += 1;
            while (index < tokens.Count && parenlevels > 0)
            {
                string token = tokens[index];
                if (tokens[index] == "(")
                {
                    parenlevels += 1;
                }

                if (tokens[index] == ")")
                {
                    parenlevels -= 1;
                }

                if (parenlevels > 0)
                {
                    subExpr.Append(token);
                }

                index += 1;
            }

            if ((parenlevels > 0))
            {
                throw new ArgumentException(string.Format(ERR_PARENTHESIS, subExpr));
            }

            return subExpr.ToString();
        }

        private static List<string> GetTokens(string expression)
        {
            string literals = "()^*/+-";
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();

            expression = expression.Replace(" ", string.Empty);
            foreach (char c in expression)
            {
                int indexOfC = literals.IndexOf(c);
                if (indexOfC == literals.IndexOf('-') && sb.Length == 0)
                {
                    sb.Append(c.ToString());
                }
                else if (indexOfC >= 0)
                {
                    if ((sb.Length > 0))
                    {
                        tokens.Add(sb.ToString());
                        sb.Length = 0;
                    }
                    tokens.Add(c.ToString());
                }
                else
                {
                    sb.Append(c.ToString());
                }
            }

            if ((sb.Length > 0))
            {
                tokens.Add(sb.ToString());
            }
            return tokens;
        }
    }
}