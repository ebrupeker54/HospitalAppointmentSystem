using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HospitalAppointmentSystem.Web.Helpers;

public class InvariantDecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);

        var text = valueResult.FirstValue?.Trim();
        if (string.IsNullOrEmpty(text))
            return Task.CompletedTask;

        text = text.Replace(',', '.');
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            bindingContext.Result = ModelBindingResult.Success(value);
        else
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Lütfen geçerli bir sayı giriniz.");

        return Task.CompletedTask;
    }
}

public class InvariantDecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.UnderlyingOrModelType == typeof(decimal)
            ? new InvariantDecimalModelBinder()
            : null;
    }
}