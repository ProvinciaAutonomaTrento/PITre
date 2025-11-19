// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System.Text.Json;

// https://abdus.dev/posts/aspnetcore-model-binding-json-query-params/

namespace Pi3.App.DocumentoAmministrativo.WebApi.Binders
{
    public class JsonQueryBinder : IModelBinder
    {
        private readonly ILogger<JsonQueryBinder> _logger;

        public JsonQueryBinder(ILogger<JsonQueryBinder> logger)
        {
            _logger = logger;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstValue;
            if (value == null)
            {
                return Task.CompletedTask;
            }

            try
            {
                //var parsed = JsonSerializer.Deserialize(
                //    value,
                //    bindingContext.ModelType,
                //    new JsonSerializerOptions(JsonSerializerDefaults.Web)
                //);
                var parsed = JsonConvert.DeserializeObject(value, bindingContext.ModelType);
                bindingContext.Result = ModelBindingResult.Success(parsed);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to bind '{FieldName}'", bindingContext.FieldName);
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
