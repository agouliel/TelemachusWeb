using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

public class CustomCorsPolicyProvider : ICorsPolicyProvider
{
    public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
    {
        var policy = new CorsPolicy();

        policy.Origins.Add("*");
        policy.Methods.Add("*");
        policy.Headers.Add("*");
        policy.SupportsCredentials = true;

        return Task.FromResult(policy);
    }
}
