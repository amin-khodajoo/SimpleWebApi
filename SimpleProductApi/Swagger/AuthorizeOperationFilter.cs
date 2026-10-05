using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleProductApi.Swagger;

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var methodAttributes = context.MethodInfo.GetCustomAttributes(true);
        var controllerAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? [];

        var hasMethodAllowAnonymous = methodAttributes.OfType<IAllowAnonymous>().Any();
        if (hasMethodAllowAnonymous)
        {
            return;
        }

        var hasMethodAuthorize = methodAttributes.OfType<IAuthorizeData>().Any();
        var hasControllerAuthorize = controllerAttributes.OfType<IAuthorizeData>().Any();
        var hasControllerAllowAnonymous = controllerAttributes.OfType<IAllowAnonymous>().Any();

        var isAuthorized = hasMethodAuthorize || (hasControllerAuthorize && !hasControllerAllowAnonymous);

        if (!isAuthorized)
        {
            return;
        }

        operation.Security ??= [];

        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            });
    }
}