﻿using Backend.Contract.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace my_project_backend.Middleware
{
    public class CustomResponseBodyMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        public async Task HandleAsync(RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (!authorizeResult.Succeeded)
            {
                try
                {
                    if (authorizeResult is { Forbidden: true }) //禁止访问
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(
                            CustomResponse<object>.Forbidden("禁止访问!"));
                    }
                    if (authorizeResult is { Challenged: true })    //未登录
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(
                            CustomResponse<object>.UnAuthorized("未登录！"));
                    }
                    if (authorizeResult.AuthorizationFailure is { FailCalled: true })   //验证错误
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(
                            CustomResponse<object>.Failure(StatusCodes.Status401Unauthorized, "用户名或密码错误！"));
                    }
                    return;
                }
                catch(InvalidOperationException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(
                        CustomResponse<object>.Failure(StatusCodes.Status400BadRequest,ex.Message));
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(
                        CustomResponse<object>.Failure(StatusCodes.Status500InternalServerError, "服务器出错了！"));
                }
            }

            await next(context);
        }
    }
}
