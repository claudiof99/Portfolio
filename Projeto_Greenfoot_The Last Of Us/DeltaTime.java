import greenfoot.*;
/**
 * Write a description of class DeltaTime here.
 * 
 * @author Joao Oliveira 
 * @version (a version number or a date)
 */
public class DeltaTime
{
    private double initTime;
    private double deltaTime;
 
    public DeltaTime()
    {
        initTime = System.currentTimeMillis();
    }
    
    public void dt()
    {
        initTime = System.currentTimeMillis();
        deltaTime = (System.currentTimeMillis() - initTime) / 1000;
    }
    
    public double deltaTime()
    {
        return deltaTime;
    }
}
