// All credit to: https://forum.unity.com/threads/keeping-entity-and-gameobject-in-sync.524237/ @primitiveconcept @tertle @IsaiahKelly

using Unity.Entities;
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
        Type component = game_object_entity.gameObject.AddComponent<Type>(); // FIXME: only add once
        if (World.Active != null)
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
    /// <typeparam name="Type">Type of Component to get.</typeparam>
    public static Type get_component_data<Type>(this GameObjectEntity game_object_entity) where Type : struct, IComponentData
    {
        if (World.Active != null)
        {
            EntityManager entity_manager = World.Active.GetExistingManager<EntityManager>();
            Entity entity = game_object_entity.Entity;
            return entity_manager.GetComponentData<Type>(entity);
        }
        // TODO: GameObject GetComponent call
        return default;
    }

    /// <summary>
    /// Inspector - Check if a Component is attached to a GameObjectEntity.
    /// </summary>
    /// <param name="game_object_entity"><see cref="GameObjectEntity"/></param>
    /// <typeparam name="Type">Type of Component to check for.</typeparam>
    public static bool has_component_data<Type>(this GameObjectEntity game_object_entity) where Type : Component
    {
        if (World.Active != null)
        {
            EntityManager entity_manager = World.Active.GetExistingManager<EntityManager>();
            Entity entity = game_object_entity.Entity;
            return entity_manager.HasComponent<Type>(entity);
        }
        // TODO: GameObject GetComponent call
        return false;
    }

    /// <summary>
    /// Remove a Component from a GameObjectEntity.
    /// </summary>
    /// <param name="game_object_entity"><see cref="GameObjectEntity"/></param>
    /// <typeparam name="Type">Type of Component to remove.</typeparam>
    public static void remove_component_data<Type>(this GameObjectEntity game_object_entity) where Type : Component
    {
        if (World.Active != null)
        {
            EntityManager entity_manager = World.Active.GetExistingManager<EntityManager>();
            Entity entity = game_object_entity.Entity;
            if (entity_manager.HasComponent<Type>(entity))
            {
                entity_manager.RemoveComponent(entity, typeof(Type));
            }
        }
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
    /// <typeparam name="Type">Type of Component to set.</typeparam>
    public static void set_component_data<Type>(this GameObjectEntity game_object_entity, Type data) where Type : struct, IComponentData
    {
        if (World.Active != null)
        {
            EntityManager entity_manager = World.Active.GetExistingManager<EntityManager>();
            Entity entity = game_object_entity.Entity;
            entity_manager.SetComponentData<Type>(entity, data);
        }
        // else TODO: GameObject GetComponent call
    }
}

// All credit to: https://forum.unity.com/threads/keeping-entity-and-gameobject-in-sync.524237/ @primitiveconcept @tertle @IsaiahKelly