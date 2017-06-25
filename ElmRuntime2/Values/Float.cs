﻿using ElmRuntime2.Exceptions;
using ElmRuntime2.Expressions;
using ElmRuntime2.Lexer;
using ElmRuntime2.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmRuntime2.Values
{
    public class Float : Number
    {
        private readonly float value;

        public Float(float value)
        {
            this.value = value;
        }

        public float Value
        {
            get { return value; }
        }

        public Expression Evaluate(Expression[] arguments, Scope scope)
        {
            return this;
        }

        public bool OperatorEquals(Expression op2)
        {
            return op2 is Float && (op2 as Float).value == value;
        }

        public bool OperatorLesserThan(Expression op2)
        {
            return op2 is Float && (op2 as Float).value < value;
        }
    }
}
