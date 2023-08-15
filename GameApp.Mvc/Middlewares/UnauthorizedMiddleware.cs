namespace GameApp.Mvc.Middlewares
{
    public class UnauthorizedMiddleware
    {
        private readonly RequestDelegate next;

        public UnauthorizedMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.Redirect("/Account/Login");
            }
            await next.Invoke(context);
        }
    }
}
