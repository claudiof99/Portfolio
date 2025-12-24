import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Text here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class Text extends Actor
{
    private String _text;
    private int _fontSize;
    
    public Text(String text, int fontSize)
    {
        this._text = text;
        this._fontSize = fontSize;
        
    };
    
    /**
     * Used to write Text making use of the setTextFont function and defining it by the parameters passed
     * @param String text - text that will be displayed
     * @param int x - x coordinate of text
     * @param int y - y coordinate of text
     * @param int fontSize - the text's font size
     * @param boolean bold - true if text is bold, false otherwise
     * @param boolean italic - true if text is italic, false otherwise
     * 
     * Function probably could have been better, my fault(Afonso)
     */
    public void writeText(String text, int x, int y, int fontSize, boolean bold, boolean italic)
    {
        GreenfootImage textImg = new GreenfootImage(text, fontSize, new Color(255,255,255), new Color(0,0,0, 124));
        setTextFont(textImg, bold, italic, fontSize);
        textImg.drawString(text, 0,0);
        setImage(textImg);
    }
    
    /**
     * Used to set the text's font based on the parameters passed
     * @param GreenfootImage text - text that will have it's font set
     * @param boolean bold - true if text is bold, false otherwise
     * @param boolean italic - true if text is italic, false otherwise
     * @param int fontSize - the text's new fontSize
     */
    public void setTextFont(GreenfootImage text, boolean bold, boolean italic, int fontSize)
    {
        Font newButtonFont = new Font(bold, italic, fontSize);
        text.setColor(Color.WHITE);
        text.setFont(newButtonFont);
    }
}
