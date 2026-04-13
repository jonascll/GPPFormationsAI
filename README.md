# AI to make basic formations
This GitHub repository consists of a demo implementation of logic for creating formations that traverse the world via navmesh.
There is 3 basic formations.
1. Rectangle formation: The formation consists of the AI forming lines in a certain number of rows. takes into account a row and column spread value
2. Circle formation: the AI forms a circle around a center point decided by a radius
3. Triangle formation: the AI forms a triangle shape, taking into account a row and column spread value
## Example GIFs of the formations in action
![](https://github.com/jonascll/GPPFormationsAI/blob/main/formationRectangle.gif)
![](https://github.com/jonascll/GPPFormationsAI/blob/main/formationCircle.gif)
![](https://github.com/jonascll/GPPFormationsAI/blob/main/formationTriangle.gif)
# Design/Implementation
## variables for formation
There is an on-screen UI I added to set all the variables needed for each formation, which consists of the following.
1. rows: the number of rows that are in the rectangle formation. Based on the number of rows, it will calculate how many columns there are and move with the navmesh to fill up all the spots it can in these rows (used in rectangle formation)
2. row spread: the distance between each row (used in rectangle formation and triangle formation)
3. column spread: the distance between each column (used in rectangle formation and triangle formation)
4. circle radius: the radius for the circle (used in circle formation)
5. column gap: this is the distance of a gap that is made in the middle of a rectangle formation (used in rectangle formation)
## Selection
I made a small script that creates a rectangle and checks for any pawns within the rectangle. This is also the way I picked the commander or controlling unit, as in the unit that controls the entire mob of AI, I picked the one closest to the start of my selection.
## Navmesh
For the AI movement, I used Unity's navmeshagent and navmeshsurface components from their navigation plugin, this allow for the AI to calculate its own path that it will move towards based on a baked navmesh. This also allows you to intervene if the path fails with certain methods that are available.
### NavMeshAgent and avoidance settings/pathing settings
The navmeshagent has multiple variables that change how the pathfinding works in regards to avoiding objects AND other navmeshagents
1. Radius: this is basically a sphere collider used to get collisions with other game objects; the radius decides the radius of this collider
2. Height: this same collider's height
3. Quality: how well the avoidance is
4. Priority: the priority of avoidance (lower number means higher priority to avoid)

The navmeshagent methods I used were
1. destination: this method sets the vector 3 destination, which the navmesh then calculates the path for, and moves the AI towards the position once it's set, without any other input
2. CalculatePath: This method gets the path that would be calculated when the destination is set with the vector target you give, which allows you to check for failed paths. I used this to go to the nearest wall when a path failed.


Some of the other settings, as seen below, change the slope you can walk on and decide the step-up height, but I didn't use these since I don't have stairs or elevations.
<br>
![](https://github.com/jonascll/GPPFormationsAI/blob/main/navmeshparameters.png)

## Rectangle formation
For the rectangle formation, I used the commander's location to calculate the destination for all the other AI selected. The index was calculated based on row and column; the columns were calculated based on the number of rows. And with these, I could now loop over each pawn that isn't the commander and calculate the destination based on the commander's position.
### Formulas used
1. index formula: Index of columns * total rows + index of rows
2. Columns formula: total pawns selected / total rows (rounded up)
3. position formula: commander position + (row Index * row spread * -commander vector forward) + (((column index * column spread) + rectangle gap) * commander vector right)
## Circle formation
The circle formation was achieved by taking the commander's position and offsetting his x coordinates by the set radius for the circle. Then, by calculating the angle needed for each pawn (in radians), I could loop over all of the pawns, excluding the commander, and calculate their destination on the circle using cos and sin.
### Formulas used
1. Angle needed: 2PI / total pawns selected
2. destination: center of circle X + (radius * cos(angle needed)) , center of circle Y + (radius * sin(angle needed))
## Triangle formation
For the triangle formation, I had multiple formulas to calculate each point on every row and column, and used these to offset the destination of each pawn from the commander's position (the commander is the tip of the triangle)
### Formulas used
1. get index of row based on index: (√8 * index + 1| -1) / 2 (rounded up)
2. get number of columns in a row: row index * (row index + 1) / 2  ( n*(n+1)/2 )
3. position formula:  X:      commander position X + ((index + 1) - column count for row) * column spread + ((row index - 1 ) * columnspread / 2)
                      Y:      commander position y + (row index - 1) * -row spread
4. rotation formula: commander rotation * (calculated position - commander position) + commander position
# Result
As seen in the demo gifs, it's a very basic system that is not fully fleshed out and still has its bugs with pathfinding, but a good demo project to learn from as a simple formation demo. The formations possible with this demo are very basic but also fundamental.
# Conclusion
This demo project is a great project to look at for the basics and to understand that even in rudimentary formations, a lot of knowledge is required to make those work. The things that could be improved in this project or taken in a different approach are, for example.
1. The selection of who the commander is , a better or different way would be to have ranks, and then you can even implement that if the highest rank unit gets destroyed, you pick the second highest unit. This could be done with a simple variable signifying the rank of a unit.
2. The destinations of the AI following the commander could also be calculated based on the pawn in the previous iteration of the loop, instead of the commander's position.
