package com.peiyh.pcedandcanon;

import android.content.Context;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.widget.RelativeLayout;

public class OneRelativeLayout extends RelativeLayout {

    public OneRelativeLayout(Context context) {
        super(context);
    }

    public OneRelativeLayout(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    //重写这个方法，并且在方法里面请求所有的父控件都不要拦截他的事件
    @Override
    public boolean dispatchTouchEvent(MotionEvent ev) {
        getParent().requestDisallowInterceptTouchEvent(true);
        return super.dispatchTouchEvent(ev);
    }
}