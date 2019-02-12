// All credit to: https://forum.unity.com/threads/keeping-entity-and-gameobject-in-sync.524237/ @primitiveconcept @tertle @IsaiahKelly

using Unity.Entities; // FIXME: remove all of these workarounds when Unity implements auto-sync of components
using UnityEngine;

/// <summary>
/// How Planetaria chooses to modify Entities (not particularly efficient or convenient, but it is based on my beginner knowledge)
/// </summary>
/// <seealso cref="https://docs.unity3d.com/Packages/com.unity.entities@0.0/api/"/>
public static class GameObjectEntityExtensions
{
    /// <summary>
    /// Add a Component to a GameObjectEntity.
    /// </summary>
    /// <param name="game_object_entity"><see cref="GameObjectEntity"/></param>
    /// <typeparam name="Type">Type of Component to add.</typeparam>
    public static void add_component_data<Type>(this GameObjectEntity game_object_entity) where Type : Component
    {
        if (!game_object_entity.gameObject.GetComponent<Type>()) // runtime and editor time
        {
            game_object_entity.gameObject.AddComponent<Type>(); // Only add component once
        }
        if (World.Active != null) // runtime only
        {
            EntityManager entity_manager = World.Active.GetExistingManager<EntityManager>();
            Entity entity = game_object_entity.Entity;
            entity_manager.AddComponent(entity, typeof(Type));
        }
    }
    
    /// <summary>
    /// Inspector - Get a Component from a GameObjectEntity.
    /// </summary>
    /// <param name="game_object_entity"><see cref="GameObjectEntity"/></param>
    /// <typeparam name="Data">Type of Data to get.</typeparam>
    /// <typeparam name="EntityComponent">Type of Data's wrapper class.</typeparam>
    public static Data get_component_data<Data, EntityComponent>(this GameObjectEntity game_object_entity) where Data : struct, IComponentData where EntityComponent : ComponentDataWrapper<Data>
    {
        if (World.Active != null) // runtime only
        {
            EntityManager entity_manager = World.Active.GetExistingManager<EntityManager>();
            Entity entity = game_object_entity.Entity;
            return entity_manager.GetComponentData<Data>(entity);
        }
        // editor time only
        EntityComponent component = game_object_entity.gameObject.GetComponent<EntityComponent>();  // Note we are dealing with structs, not classes (and apparently this doesn't work anyway =/ )
        if (component) // Figure out if the component is attached
        {
            return component.Value;
        }
        return default; // otherwise return default
    }

    /// <summary>
    /// Inspector - Check if a Component is attached to a GameObjectEntity.
    /// </summary>
    /// <param name="game_object_entity"><see cref="GameObjectEntity"/></param>
    /// <typeparam name="Type">Type of Component to check for.</typeparam>
    public static bool has_component_data<Type>(this GameObjectEntity game_object_entity) where Type : Component
    {
        if (World.Active != null) // runtime only
        {
            EntityManager entity_manager = World.Active.GetExistingManager<EntityManager>();
            Entity entity = game_object_entity.Entity;
            return entity_manager.HasComponent<Type>(entity);
        }
        // editor time only
        Type[] component = game_object_entity.gameObject.GetComponents<Type>();  // Note we are dealing with structs, not classes
        return component.Length != 0; // Determine if the component exists on the game object
    }

    /// <summary>
    /// Remove a Component from a GameObjectEntity.
    /// </summary>
    /// <param name="game_object_entity"><see cref="GameObjectEntity"/></param>
    /// <typeparam name="Type">Type of Component to remove.</typeparam>
    public static void remove_component_data<Type>(this GameObjectEntity game_object_entity) where Type : Component
    {
        if (World.Active != null) // runtime only
        {
            EntityManager entity_manager = World.Active.GetExistingManager<EntityManager>();
            Entity entity = game_object_entity.Entity;
            if (entity_manager.HasComponent<Type>(entity))
            {
                entity_manager.RemoveComponent(entity, typeof(Type));
            }
        }
        // editor time and runtime
        if (game_object_entity.GetComponent<Type>())
        {
            GameObject.Destroy(game_object_entity.GetComponent<Type>());
        }
    }

    /// <summary>
    /// Mutator - Set a Component from a GameObjectEntity.
    /// </summary>
    /// <param name="game_object_entity"><see cref="GameObjectEntity"/></param>
    /// <param name="data">Component data to replace current data.</param>
    /// <typeparam name="Data">Type of Data to get.</typeparam>
    /// <typeparam name="EntityComponent">Type of Data's wrapper class.</typeparam>
    public static void set_component_data<Data, EntityComponent>(this GameObjectEntity game_object_entity, Data data) where Data : struct, IComponentData where EntityComponent : ComponentDataWrapper<Data>
    {
        if (World.Active != null) // runtime only
        {
            EntityManager entity_manager = World.Active.GetExistingManager<EntityManager>();
            Entity entity = game_object_entity.Entity;
            entity_manager.SetComponentData<Data>(entity, data);
        }
        else // editor time only
        {
            EntityComponent component = game_object_entity.gameObject.GetComponent<EntityComponent>();  // Note we are dealing with structs, not classes
            if (component) // Figure out if the component is attached
            {
                component.Value = data; // Set the internal data
            }
        }
    }
}

// All credit to: https://forum.unity.com/threads/keeping-entity-and-gameobject-in-sync.524237/ @primitiveconcept @tertle @IsaiahKelly