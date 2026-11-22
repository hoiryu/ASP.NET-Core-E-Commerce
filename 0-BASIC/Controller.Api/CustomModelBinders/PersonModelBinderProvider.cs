using Controller.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Controller.Api.CustomModelBinders;

public class PersonModelBinderProvider : IModelBinderProvider
{
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		if (context.Metadata.ModelType == typeof(Person))
			return new BinderTypeModelBinder(typeof(PersonModelBinder));

		return null;
	}
}
