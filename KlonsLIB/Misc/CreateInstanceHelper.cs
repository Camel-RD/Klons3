using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KlonsLIB.Misc;

public static class CreateInstanceHelper
{
    public static Func<object> CreateFunc<T>()
    {
        var type = typeof(T);
        var constructorTypes = new List<Type>();
        var parameters = new List<ParameterExpression>();
        var constructor = type.GetConstructor(constructorTypes.ToArray());
        var constructorParameters = parameters;
        var newExpr = Expression.New(constructor, constructorParameters);
        var lambdaExpr = Expression.Lambda<Func<object>>(newExpr, parameters);
        var func = lambdaExpr.Compile();
        return func;
    }

    public static Func<TArg1, object> CreateFunc<T, TArg1>()
    {
        var type = typeof(T);
        var constructorTypes = new List<Type>()
        {
            typeof(TArg1)
        };
        var parameters = new List<ParameterExpression>()
        {
          Expression.Parameter(typeof(TArg1)),
        };

        var constructor = type.GetConstructor(constructorTypes.ToArray());
        var constructorParameters = parameters.Take(constructorTypes.Count).ToList();
        var newExpr = Expression.New(constructor, constructorParameters);
        var lambdaExpr = Expression.Lambda<Func<TArg1, object>>(newExpr, parameters);
        var func = lambdaExpr.Compile();
        return func;
    }

    public static Func<TArg1, TArg2, T> CreateFunc<T, TArg1, TArg2>()
    {
        var type = typeof(T);
        var constructorTypes = new List<Type>()
        {
            typeof(TArg1),
            typeof(TArg2),
        };
        var parameters = new List<ParameterExpression>()
        {
          Expression.Parameter(typeof(TArg1)),
          Expression.Parameter(typeof(TArg2)),
        };

        var constructor = type.GetConstructor(constructorTypes.ToArray());
        var constructorParameters = parameters.Take(constructorTypes.Count).ToList();
        var newExpr = Expression.New(constructor, constructorParameters);
        var converExpr = Expression.Convert(newExpr, type);
        var lambdaExpr = Expression.Lambda<Func<TArg1, TArg2, T>>(converExpr, parameters);
        var func = lambdaExpr.Compile();
        return func;
    }
}
