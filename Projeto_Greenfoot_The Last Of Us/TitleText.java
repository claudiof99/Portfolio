import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class TitleText here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class TitleText extends Text
{
    /**
     * Instantiates the title text that is displayed on the title screen
     * 
     * Probably shouldn't have been a subclass, doesn't matter now teacher
     */
    public TitleText()
    {
        super("The Last of Us", 110);
        
        writeText("The Last of Us",450, 300, 110, true, false);
    };
}
