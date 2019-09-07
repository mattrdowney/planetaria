using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingVolumeHierarchy
{
    // A cone, sphere, or axis-alligned bounding box are all examples of volumes. (Calling it a Bounding Cone Hierarchy would be stupid)
    // Here I use either a cone, a circle, or the volume on one side of an infinite plane (depending on your perspective).
    // Unlike other bounding volume hierarchies, I think it's easier to make a good partition.
    // Step 1: pick a random partition vector. (Perpendicular to last partition vector, if applicable.)
    // Step 2: Sort the left and right boundaries along that partition vector
    // Step 3: Alternate between the leftmost and rightmost elements, taking greedily into partition cluster until you have a perfect 50-50 split (off by one when odd).
    // Step 4: Go back to Step 1 if there are any remaining partitions required.

    // The basic structure on each level is two infinite planes (in abstract).
    // Dot products are the main operation used on each level.
    // Stored data on each level: one vector and two floats (implicit second vector points in the opposite direction)

    // Why does this work so well? Unlike standard worlds, everything is contained in a single unit-sphere, so partition lines don't really need smart positions -- you can derive those easily via the sorting method (mostly just smart directions)

    // Another concept:
    // let's say you have an equator line.
    // Up until now I was strongly considering taking the designer (or tooling system) and making them split those equator lines into several composite lines.
    // I forgot there's an easier way to do it.
    // Keep one line but break up the collider into multiple colliders up to a certain radius. Make those colliders point to the same object.
    // Although multiple collision checks are possible, more than two is pretty unlikely with discrete collision checks at runtime unless the dynamic collider is large
    // Multiple collision checks are more likely with raycasting, but since you avoid work as well the benefits probably outweigh the negatives.

    // Raycasting should be interesting, you basically want to sweep in a circle until you intersect with a plane (after which you have to do extra steps).
    // There are possibilities of early returns, but you have to be careful not to return too early.
    // Most likely, you can use a stack (data structure), similar to typical Cartesian bounding volume hierarchies.

    // Another "optimization":
    // Let's say you pick a bad random partition direction.
    // Save the results -- namely the direction and the two floats (just in case they weren't actually that bad).
    // Try a few more partitions and see if any of them do better.

    // Another "optimization":
    // Choose a partition based on a random direction perpendicular to the center of mass of the cluster.
    // One extreme outlier can make a bad partition but it's probably better than random partition lines anyway.
    // Actually, if you do the center of mass in a Cartesian system you could have 50 points (0,0,0), 50 points (0,0,1) and 1 point (0,0,1000) and the partition would be ruined.
    // In spherical coordinates you have a normalized vector, so the weights are considered "equally".
    // The main thing that isn't considered fairly is the radius of the point.

    // Also, I'm going to use this as a (major) reference: https://github.com/brandonpelfrey/Fast-BVH
}
