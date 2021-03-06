﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SymbolicMath.ExpressionHelper;

namespace SymbolicMath
{
    /// <summary>
    /// A function is an <see cref="Expression"/> that has one argument.
    /// </summary>
    /// <remarks>
    /// When extending this class, remember to pass in the constant value to the constructor <see cref="Function(Expression, double)"/>.
    /// Also, implement the <see cref="Expression.Derivative(string)"/> and <see cref="Expression.Evaluate(Dictionary{string, double})"/> methods.
    /// </remarks>
    public abstract class Function : Expression
    {
        public Expression Argument { get; }

        public override bool IsConstant { get; }

        public override int Height { get; }

        public override int Size { get; }

        public override int Complexity { get; }

        private readonly double m_value;

        private readonly int m_hashCode;

        public override double Value
        {
            get
            {
                if (!IsConstant)
                {
                    throw new InvalidOperationException("This Function is not constant");
                }
                else
                {
                    return m_value;
                }
            }
        }

        protected Function(Expression arg, double value)
        {
            Argument = arg;
            IsConstant = arg.IsConstant;
            Height = arg.Height + 1;
            Size = arg.Size + 1;
            Complexity = arg.Complexity + 1;
            m_value = value;
            m_hashCode = base.GetHashCode() ^ arg.GetHashCode();
        }

        public abstract Expression With(Expression arg);

        public override Expression With(IReadOnlyDictionary<Variable, Expression> values)
        {
            return this.With(Argument.With(values));
        }

        public override bool Equals(object obj)
        {
            return (obj.GetType() == this.GetType()) && (obj as Function).Argument.Equals(this.Argument);
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }
    }

    public class Negative : Function
    {
        public override int Complexity { get { return Argument.Complexity; } }
        internal Negative(Expression arg) : base(arg, (arg.IsConstant) ? -arg.Value : 0) { }

        internal override Expression DerivativeInternal(Variable variable)
        {
            return -Argument.DerivativeInternal(variable);
        }

        public override double Evaluate(IReadOnlyDictionary<Variable, double> context)
        {
            return -Argument.Evaluate(context);
        }

        public override Expression With(Expression arg)
        {
            return arg.Neg();
        }

        public override Expression Neg()
        {
            return Argument;
        }

        public override string ToString()
        {
            return $"(-{Argument})";
        }
    }

    public class Invert : Function
    {
        internal Invert(Expression arg) : base(arg, arg.IsConstant ? 1 / arg.Value : 0) { }

        internal override Expression DerivativeInternal(Variable variable)
        {
            Expression f = Argument;
            return -(f ^ -2) * f.DerivativeInternal(variable);
        }

        public override double Evaluate(IReadOnlyDictionary<Variable, double> context)
        {
            return 1.0 / Argument.Evaluate(context);
        }

        public override Expression With(Expression arg)
        {
            return arg.Inv();
        }

        public override Expression Inv()
        {
            return Argument;
        }

        public override string ToString()
        {
            return $"(1/{Argument.ToString()})";
        }
    }

    public class Exponential : Function
    {
        internal Exponential(Expression arg) : base(arg, (arg.IsConstant) ? Math.Exp(arg.Value) : 0) { }

        internal override Expression DerivativeInternal(Variable variable)
        {
            return this * Argument.DerivativeInternal(variable);
        }

        public override double Evaluate(IReadOnlyDictionary<Variable, double> context)
        {
            return Math.Exp(Argument.Evaluate(context));
        }

        public override Expression With(Expression arg)
        {
            return arg.Exp();
        }

        public override Expression Log()
        {
            return Argument;
        }

        public override string ToString()
        {
            return $"e^({Argument})";
        }
    }

    public class Logarithm : Function
    {
        internal Logarithm(Expression arg) : base(arg, (arg.IsConstant) ? Math.Log(arg.Value) : 0) { }

        internal override Expression DerivativeInternal(Variable variable)
        {
            return Argument.DerivativeInternal(variable) / Argument;
        }

        public override double Evaluate(IReadOnlyDictionary<Variable, double> context)
        {
            return Math.Log(Argument.Evaluate(context));
        }

        public override Expression With(Expression arg)
        {
            return arg.Log();
        }

        public override Expression Exp()
        {
            return Argument;
        }

        public override string ToString()
        {
            return $"ln({Argument})";
        }
    }

    public class Sine : Function
    {
        internal Sine(Expression arg) : base(arg, (arg.IsConstant) ? Math.Sin(arg.Value) : 0) { }

        internal override Expression DerivativeInternal(Variable variable)
        {
            return Argument.Cos() * Argument.DerivativeInternal(variable);
        }

        public override double Evaluate(IReadOnlyDictionary<Variable, double> context)
        {
            return Math.Sin(Argument.Evaluate(context));
        }

        public override Expression With(Expression arg)
        {
            return arg.Sin();
        }

        public override string ToString()
        {
            return $"sin({Argument})";
        }
    }

    public class Cosine : Function
    {
        internal Cosine(Expression arg) : base(arg, (arg.IsConstant) ? Math.Cos(arg.Value) : 0) { }

        internal override Expression DerivativeInternal(Variable variable)
        {
            return -Argument.Sin() * Argument.DerivativeInternal(variable);
        }

        public override double Evaluate(IReadOnlyDictionary<Variable, double> context)
        {
            return Math.Cos(Argument.Evaluate(context));
        }

        public override Expression With(Expression arg)
        {
            return arg.Cos();
        }

        public override string ToString()
        {
            return $"cos({Argument})";
        }
    }

    public class Tangent : Function
    {
        internal Tangent(Expression arg) : base(arg, (arg.IsConstant) ? Math.Tan(arg.Value) : 0) { }

        internal override Expression DerivativeInternal(Variable variable)
        {
            Expression Cos = Argument.Cos();
            return (Cos * Cos).Inv() * Argument.DerivativeInternal(variable);
        }

        public override double Evaluate(IReadOnlyDictionary<Variable, double> context)
        {
            return Math.Tan(Argument.Evaluate(context));
        }

        public override Expression With(Expression arg)
        {
            return arg.Tan();
        }

        public override string ToString()
        {
            return $"tan({Argument})";
        }
    }
}
