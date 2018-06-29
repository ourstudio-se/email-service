using System;
using System.IO;
using System.Threading.Tasks;
using EmailService.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace EmailService.Service.Implementations
{
	public class HtmlGeneratorService : IHtmlGeneratorService
	{
		private readonly IRazorViewEngine _razorViewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IServiceProvider _serviceProvider;
		
		public HtmlGeneratorService(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider,
			IServiceProvider serviceProvider)
		{
			_razorViewEngine = razorViewEngine;
			_tempDataProvider = tempDataProvider;
			_serviceProvider = serviceProvider;
		}
		
		public async Task<string> GetRawHtmlAsync(string view, EmailViewModel viewModel)
		{
			// Inspired by: https://stackoverflow.com/a/40932984
			
			DefaultHttpContext defaultHttpContext = new DefaultHttpContext() { RequestServices = _serviceProvider };
			ActionContext actionContext = new ActionContext(defaultHttpContext, new RouteData(), new ActionDescriptor());
			ViewDataDictionary viewDataDictionary =
				new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = viewModel };

			ViewEngineResult result = _razorViewEngine.FindView(actionContext, view, false);

			bool hasNoResult = result.View == null;

			if (hasNoResult)
			{
				return null;
			}

			using (StringWriter stringWriter = new StringWriter())
			{
				ViewContext viewContext = new ViewContext(actionContext, result.View, viewDataDictionary,
					new TempDataDictionary(actionContext.HttpContext, _tempDataProvider), stringWriter, new HtmlHelperOptions());

				await result.View.RenderAsync(viewContext);

				return stringWriter.ToString();
			}
		}
	}
}