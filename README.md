# AI to make basic formations
This github repository consists of a demo implementation of logic for creating formations that traverse the world via navmesh
There is 3 basic formations
1. Rectangle formation: the formation consists of the ai forming lines in a certain ammount of rows
2. Circle formation: the ai forms a circle around a center point decided by a radius
3. Triangle formation: the ai forms a triangle shape
## Example GIF's of the formations in action
![](https://github.com/jonascll/GPPFormationsAI/blob/main/formationRectangle.gif)
![](https://github.com/jonascll/GPPFormationsAI/blob/main/formationCircle.gif)
![](https://github.com/jonascll/GPPFormationsAI/blob/main/formationTriangle.gif)
# Design/Implementation
## variables for formation
There is a on screen UI i added to set all the variables needed for each formation, these exist of the following
1. rows: the ammount of rows that are in the rectangle formation, based on the ammount of rows it will calculate how many collumns there are and move with the navmesh to fill up all the spots it can in these rows (used in rectangle formation)
2. row spread : the distance between each row (used in rectangle formation and triangle formation)
3. column spread : the distance between each column (used in rectangle formation and triangle formation)
4. circle radius : the radius for the circle (used in circle formation)
5. column gap: this is the distance of a gap that is made in the middle of a rectangle formation (used in rectangle formation)
## Selection
i made a small script that makes a rectangle and check for any pawns in given rectangle, this is also the way i picked the commander or controling unit, as in the unit that controls the entire mob of ai, i picked the one closest to the start of my selection.
## Navmesh
for the ai movement i used unity's navmeshagent and navmeshsurface components from their navigation plugin, this allow for the ai to calculates its own path that it will move towards based on a baked navmesh. this also allows you to do intervene if the path fails with certain methods that are available
### NavMeshAgent and avoidance settings/pathing settings
the navmeshagent  has multiple variables that change how the pathfinding works in regards to avoiding objects AND other navmeshagents
1. radius : this is basically a sphere collider used to get collisions with other game objects, radius decides the radius of this collider
2. height : this same collider's height
3. quality : how good the avoiding is
4. priority : the priority of avoidance (lower number means higher priority to avoid)

The navmeshagent methods i used were
1. destination : this method sets the vector 3 destination which the navmesh then calculates the path for and moves the ai towards the position once its set without any other input
2. CalculatePath : this method gets the path that would be calculated when destination is set with the vector target you give, this allows you to check for failed paths. I used this to go to the nearest wall when a path failed


some of the other setting as seen below change the slope you can walk on and decide the step up height but i didnt use these since i dont have stairs or elevations
<br>
![](https://github.com/jonascll/GPPFormationsAI/blob/main/navmeshparameters.png)

## Rectangle formation
For the rectangle formation i used the commanders location to calculate the destination for all the other ai selected, the index was calculated based on row and column, the columns were calculated based on the ammount of rows. And with these i could now loop over each pawn that isnt the commander and calculate the destination based on the comanders position
### Formulas used
1. index formula : Index of columns * total rows + index of rows
2. Columns formula : total pawns selected / total rows (rounded up)
## Circle formation
The circle formation was achieved by taking the comanders position and offsetting his x coordinates by the set radius for the circle, then by calculating the angle needed for each pawn (in radians), i could then loop over all of the pawns excluding the commander and calculate their destination  on the circle using cos and sin
### Formulas used
1. Angle needed : 2PI / total pawns selected
2. destination : center of circle X + (radius * cos(angle needed)) , center of circle Y + (radius * sin(angle needed))
## Triangle formation
for the triangle formation i had multiple formulas to calculate each point on every row and column and used these to offset the destination of each pawn from the commanders position (comander is the tip of the triangle)
### Formulas used
1. get index of row based on index : (√8 * index + 1| -1) / 2 (rounded up)
2. get number of columns in a row : row index * (row index + 1) / 2  ( n*(n+1)/2 )
# Result
As seen in the demo gif's its a very basic system that is not fully fledged out and still has its bugs with pathfinding but a good demo project to learn from as a simple formation demo, the formations possible with this demo are very basic but also fundamental
# Conclusion
this demo project is a great project to look at for the basic and to understand that even in rudamentary formations a lot of knowledge is required to make those work, the things that could be improved in this project or taken in a different approach are for example
1. The selection of who the commander is , a better or different way would be to have ranks and then you can even implement that if the highest rank unit gets destroyed you pick the second highest unit, this could be done with a simple variable signifying rank of a unit
2. the destionations of the AI following the commander could also be calculated based on the pawn in the previous iteration of the loop instead of the commanders position
