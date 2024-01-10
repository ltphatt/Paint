# Paint - Windows Programming
## Team members
- 21120514 - Nguyễn Thành Nhân
- 21120519 - Lê Thanh Phát
- 21120592 - Nguyễn Xuân Vi

## Technical details
- Design pattern: Prototype

## Core features
1. Dynamically load all graphic objects that can be drawn from external DLL files
2. The user can choose which object to draw
3. The user can see the preview of the object they want to draw
4. The user can finish the drawing preview and their change becomes permanent with previously drawn objects
5. The list of drawn objects can be saved and loaded again for continuing later. We save in json format
6. Save and load all drawn objects as an image in bmp/png/jpg format (rasterization).

## Basic graphic objects
1. Line: controlled by two points, the starting point, and the endpoint
2. Rectangle: controlled by two points, the left top point, and the right bottom point
3. Square: controlled by two points, the left top point, and the right bottom point
4. Ellipse: controlled by two points, the left top point, and the right bottom point
5. Circle: controlled by two points, the left top point, and the right bottom point
6. Right triangle: controled by two points, the left top point, and the right bottom point

## Improvements
1. Allow the user to change the color, pen width, stroke type
2. Adding image to the canvas
3. Undo, Redo
4. Three bonus graphic objects: square, circle, triangle

## Unimplemented features
1. Adding text to the list of drawable objects
2. Cut / Copy / Paste
3. Zooming
4. Select a single element for editing again

## Self-evaluation rating (Maximum is 10pts)
- 21120514 - Nguyễn Thành Nhân: 9
- 21120519 - Lê Thanh Phát: 9.5
- 21120592 - Nguyễn Xuân Vi: 9

## Demo project
- Link:





