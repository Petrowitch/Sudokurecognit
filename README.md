# Sudokurecognit

## Project overview

This project was developed for debug purposes. Åhe main objective of this project is to examine and debug Sudoku recognition algorithm. Algorithm develops for mobile devices. 
C# was selected, because project starts when started to learn this language.

*Input data*: an image (.BMP, .jPEG or .gif)
- first of all algorith search perpendicular lines
- try to correct angle. and chop borders. 
- marks square grid 9x9 
- after all try to reconize every number in every cell. (have 2 algorithm )

*Output data*: 2d array of numbers where
**0** - is no number in cell
**1-9** - this number in cell

## Known bugs

- sometimes dont recognit numbers. '6' '8' '9' and '3'
- problems with grid
- dont have yet any exception. any error in algorith invoke fatal error and finish th programm.

## Future plans 

- add exceptions
- complete algorithm of number recognition by outline;
- Add neural network to number recognition.

transfer algorithm into mobile platform.


