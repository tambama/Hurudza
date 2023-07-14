﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hurudza.Apis.Base.Models;

public class ApiBadRequestResponse : ApiResponse
{
    public IEnumerable<string> Errors { get; }

    public ApiBadRequestResponse(ModelStateDictionary modelState, string? message = null)
        : base(400, message)
    {
        if (modelState.IsValid)
        {
            throw new ArgumentException("ModelState must be invalid", nameof(modelState));
        }

        Errors = modelState.SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();
    }
}