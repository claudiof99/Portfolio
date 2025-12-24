import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Ground here.
 * 
 * @author Joao Oliveira 
 * @version (a version number or a date)
 */
public class Ground extends Platform
{
    // image of the ground
    public GreenfootImage _sprite;
    
    /**
     * Constructor for the Ground class
     * Sets the ground's image and scales
     */
    public Ground()
    {
        _sprite = new GreenfootImage("./Platforms/ground_test2.png");
        _sprite.scale(1000,100);
        setImage(_sprite);
    }
}
