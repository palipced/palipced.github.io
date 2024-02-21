package com.peiyh.pcedpalidict192;

import androidx.annotation.ColorRes;

public class Fruit {
    private  String rootid;
    private  String filename;
    private  String txt;
    private  String tooltip;
    private  String ISLEAF;
    private  String PIANPARENT;

    private String ID;
    private String name;
    private int imageId;
    private int imageinmuluId;
    private @ColorRes int backcolor;

    public  Fruit(String rootid,String filename,String txt,String tooltip,String ISLEAF,String PIANPARENT,String ID,String name,int imageId,int imageinmuluId,@ColorRes int backcolor){
        this.rootid=rootid;
        this.filename=filename;
        this.txt=txt;
        this.tooltip=tooltip;
        this.ISLEAF=ISLEAF;
        this.PIANPARENT=PIANPARENT;

        this.ID=ID;
        this.name=name;
        this.imageId=imageId;
        this.imageinmuluId=imageinmuluId;
        this.backcolor=backcolor;
    }

    public String getRootid(){
        return rootid;
    }

    public String getFilename(){
        return filename;
    }

    public String getTxt(){
        return txt;
    }

    public String getTooltip(){
        return tooltip;
    }

    public String getISLEAF(){
        return ISLEAF;
    }

    public String getPIANPARENT(){
        return PIANPARENT;
    }





    public String getID(){
        return ID;
    }

    public String getName(){
        return name;
    }

    public int getimageId(){
        return imageId;
    }

    public int getimageinmuluId(){
        return imageinmuluId;
    }

    public @ColorRes int getBackcolor(){
        return backcolor;
    }
}
