package com.peiyh.PaliTipitakaDict;

import android.content.Context;
import androidx.annotation.Nullable;
import android.util.AttributeSet;
import android.widget.TextView;

//是否应继承自v7?
public class MLTextView extends TextView {
    public String MLbookname = "";
    public String MLfilename = "";

    public MLTextView(Context context) {
        this(context, null);
        this.setTextColor(getResources().getColor(android.R.color.holo_blue_bright));
    }

    public MLTextView(Context context, @Nullable AttributeSet attrs) {
        this(context, attrs, 0);
        this.setTextColor(getResources().getColor(android.R.color.holo_blue_bright));
    }

    public MLTextView(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
        this.setTextColor(getResources().getColor(android.R.color.holo_blue_bright));
    }

    @Override
    public boolean performClick() {
        this.setTextColor(getResources().getColor(android.R.color.holo_red_dark));
        return super.performClick();
    }

}
