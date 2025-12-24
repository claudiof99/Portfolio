import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Level here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */ 
public class Level extends World
{
    private int _currentLVL;
    private String _backgroundIMG;
    private String _levelName;
    /**
     * Constructor for objects of class Level.
     * 
     */
    ///int currentLVL, String filePath, String filePathFLOOR
    public Level()
    {
        super(900, 700, 1);
    }
    
    // public static Gun createGun(int projectileSpeed, String filePath)
    // {
        // Gun gun = new Gun(projectileSpeed, filePath);
        // return gun;
    // }
    
    // public static GunGlow createGlow()
    // {
        // GunGlow glow = new GunGlow();
        // return glow;
    // }
}