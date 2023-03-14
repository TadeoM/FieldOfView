# FieldOfView
Unity Demo Game can be found here: https://gamebaking.com/prototypes/Summer2020Prototype/

This Field of View does the following:
- takes a sphere cast of all objects in the field of view (in this case 360 degrees)
- I take the vertices of the objects, and traingulate a mesh that goes along only the vertices that are visible to the player
    - Using raycasts we determine if the vertex is reachable
- the one constraint I add is that if there is no object in the way, then the field of view is at max range for a short segment.
- once all the vertices are calculated, a mesh is created.
- the mesh is shoved into a shader calculation that then only renders the objects that are within this field of view.

This field of view was created for a player, and so there was no requirement for determining whether an object is visible, but extrapolating this 
to include wouldn't be complicated, I believe the complicated part is forming the algorithm to calculate the mesh. Once I have all the objects that
are visible inside the mesh, a meshcast would have provided an elegant array of visible objects.
