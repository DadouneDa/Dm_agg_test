using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DmAggregator.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class FluentValidationConfigurationExtensions
    {
        /// <summary>
        /// Binds the configuration to type T and validates it by searching for a matching <see cref="AbstractValidator{T}"/>.
        /// If validates then a value is added as singleton.
        /// This is better than using <see cref="IOptions{TOptions}"/> because this will fail during startup, as opposed to the latter
        /// that will fail only when first used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns>The singleton instance</returns>
        public static T FluentValidateAndAddSingleton<T>(this IServiceCollection services, IConfiguration configuration) where T : class, new()
        {
            var instance = configuration.FluentValidate<T>();

            services.AddSingleton(instance);

            return instance;
        }

        /// <summary>
        /// Binds the configuration to type T and validates it by searching for a matching <see cref="AbstractValidator{T}"/>.
        /// This is better than using <see cref="IOptions{TOptions}"/> because this will fail during startup, as opposed to the latter
        /// that will fail only when first used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static T FluentValidate<T>(this IConfiguration configuration) where T : class, new()
        {
            // Note - use this method of creating a default instance and binding it, instead of configuration.Get<T>() 
            // Because this allows the configuration to be empty (default property values must be defined in class!)
            // With Get<T> an empty configuration fails!
            var instance = new T();
            configuration.Bind(instance);

            Type abstractValidatorType = typeof(AbstractValidator<T>);

            Type? validatorType = Assembly.GetCallingAssembly().GetTypes()
                .Where(t => t.IsAssignableTo(abstractValidatorType))
                .FirstOrDefault();

            if (validatorType == null)
            {
                throw new ApplicationException($"Cannot find validator of type '{typeof(AbstractValidator<T>)}'");
            }

            AbstractValidator<T> validator = (AbstractValidator<T>)Activator.CreateInstance(validatorType)!;

            validator.ValidateAndThrow(instance);

            return instance;
        }

    }
}
