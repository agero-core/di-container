#DI Container

[![NuGet Version](http://img.shields.io/nuget/v/Agero.Core.DIContainer.svg?style=flat)](https://www.nuget.org/packages/Agero.Core.DIContainer/) 
[![NuGet Downloads](http://img.shields.io/nuget/dt/Agero.Core.DIContainer.svg?style=flat)](https://www.nuget.org/packages/Agero.Core.DIContainer/)

Dependency injection library for .NET applications.

* Create container
```csharp
var container = ContainerFactory.Create();
```

* Register dependency in container and specify object lifetime
  * Register object
  ```csharp
  container.RegisterInstance<IVehicle>(new Vehicle());
  ```  
  * Register type
  ```csharp
  container.RegisterImplementation<IVehicle, Vehicle>(Lifetime.PerCall);
  ```
  * Register factory
  ```csharp
  container.RegisterFactory<IVehicle>(c => new Vehicle(), Lifetime.PerContainer);
  ```
 
* Get object from container
```csharp
var vehicle = container.Get<IVehicle>();
```

* Use container auto injection by applying **[Inject]** attribute.
  * Inject using constructor
  ```csharp
    public class Vehicle : IVehicle
    {
        [Inject]
        public Vehicle(IMake make)
        {
            Make = make;
        }
        
        public IMake Make { get; }
    }
  ```
  * Inject using property
  ```csharp
    public class Vehicle : IVehicle
    {
        [Inject]
        public IMake Make { get; set; }
    }
  ```

* Inject container itself if required by using **IReadOnlyContainer** or **IContainer** interfaces
```csharp
public class Vehicle : IVehicle
{
   [Inject]
   public Vehicle(IReadOnlyContainer container)
   {
       Container = container;
   }
   
   public IReadOnlyContainer Container { get; }
}
```      



