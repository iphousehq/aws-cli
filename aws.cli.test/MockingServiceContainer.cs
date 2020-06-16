using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightInject;
using Moq;
using Moq.Internals;
using Type = System.Type;

namespace Comsec.Aws
{
    /// <summary>
    /// Service container that automatically mocks dependencies!
    /// https://github.com/apjanes/LightInject.AutoMoq/blob/master/src/MockingServiceContainer.cs
    /// </summary>
    public class MockingServiceContainer : ServiceContainer
    {
        private readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> mocks = new Dictionary<Type, object>();

        /// <summary>
        /// Default constructor (disabling property injection).
        /// </summary>
        public MockingServiceContainer() : base(o => o.EnablePropertyInjection = false)
        {
            RegisterFallback(CanResolveMock, ResolveMock);
        }

        /// <summary>
        /// Constructor with container options.
        /// </summary>
        public MockingServiceContainer(Action<ContainerOptions> configureOptions) : base(configureOptions)
        {
            RegisterFallback(CanResolveMock, ResolveMock);
        }

        public Mock<T> GetMock<T>() where T : class
        {
            return (Mock<T>)GetMock(typeof(T));
        }

        public Mock GetMock(Type serviceType)
        {
            CreateMockIfRequired(serviceType);
            return (Mock)Get(mocks, serviceType);
        }

        private object ResolveMock(ServiceRequest request)
        {
            CreateMockIfRequired(request.ServiceType);
            return Get(instances, request.ServiceType);
        }

        private bool CanResolveMock(Type serviceType, string name)
        {
            return true;
        }

        private void CreateMockIfRequired(Type serviceType)
        {
            if (instances.ContainsKey(serviceType))
                return;
            CreateMock(serviceType);
        }

        /// <summary>
        /// Create a mock for the specified type.  If a mock of a concrete type is created, the
        /// child properties of the object are also resolved either as concretes from the container
        /// or as mocks.
        /// 
        /// Created mocks have their underlying object instance extracted and both the mock and the
        /// instance are added to the container's dictionaries.
        /// </summary>
        /// <param name="serviceType">The type of mock to create.</param>
        private void CreateMock(Type serviceType)
        {
            Type mockType;
            try
            {
                mockType = typeof(Mock<>).MakeGenericType(serviceType);
            }
            catch (ArgumentException)
            {
                // There could have been an invalid type that does not meet the type
                // constraints.  Just continue.
                return;
            }

            object mock;
            try
            {
                mock = ConstructMock(mockType, serviceType);
            }
            catch (TargetInvocationException)
            {
                // Some types cannot be mocked and so will throw an exception.
                // Ignore these, leaving their properties as null.
                return;
            }

            var instance = ExtractObject(mock, serviceType);
            instances.Add(serviceType, instance);
            mocks.Add(serviceType, mock);

            if (instance is InterfaceProxy)
                // Do not set properties on interfaces.  They are done via mock setups
                return;

            var instanceType = instance.GetType();
            var instanceProps = instanceType.GetProperties()
                                            .Where(x => x.CanRead && x.CanWrite);

            foreach (var prop in instanceProps)
            {
                var current = prop.GetValue(instance, new object[0]);
                if (current == null)
                {
                    var propInstance = GetInstance(prop.PropertyType);
                    prop.SetValue(instance, propInstance, new object[0]);
                }
            }
        }

        private object ConstructMock(Type mockType, Type serviceType)
        {
            var constructor = serviceType.GetConstructors()
                                         .OrderBy(x => x.GetParameters().Length)
                                         .FirstOrDefault();

            var parameterInstances = new List<object>();
            if (constructor != null)
            {
                var parameterInfos = constructor.GetParameters();
                foreach (var parameterInfo in parameterInfos)
                {
                    if (parameterInfo.ParameterType == serviceType)
                    {
                        // There is a circular reference, break
                        break;
                    }

                    try
                    {
                        var parameterInstance = GetInstance(parameterInfo.ParameterType);
                        parameterInstances.Add(parameterInstance);
                    }
                    catch (ArgumentException)
                    {
                        // if this is an invalid type for resolution, just continue                        
                    }
                }

            }
            return Activator.CreateInstance(mockType, parameterInstances.ToArray());
        }

        private object Get(Dictionary<Type, object> dictionary, Type serviceType)
        {
            return dictionary.ContainsKey(serviceType)
                ? dictionary[serviceType]
                : null;
        }

        /// <summary>
        /// Extracts the underlying object from the supplied Moq Mock object.
        /// </summary>
        /// <param name="mock">The Mock to extract the object from.</param>
        /// <param name="serviceType">The expected type of the object being extracted.</param>
        /// <returns>The underlying object (from Mock[T].Object)"></returns>
        private object ExtractObject(object mock, Type serviceType)
        {
            var objProp = mock.GetType()
                              .GetProperties()
                              .First(x => x.PropertyType == serviceType && x.Name == "Object");

            return objProp.GetValue(mock, new object[0]);
        }
    }
}