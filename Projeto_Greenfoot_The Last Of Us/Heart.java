import greenfoot.*;

 /** Write a description of class Heart here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class Heart extends Actor
{
    /**
     * Act - do whatever the Heart wants to do. This method is called whenever
     * the 'Act' or 'Run' button gets pressed in the environment.
     */
    private GreenfootImage _sprite;
    public Heart() 
    {
      _sprite= new GreenfootImage("./Extras/heart.png");
      this._sprite.scale(25,25);
      setImage(_sprite);
    }    
}