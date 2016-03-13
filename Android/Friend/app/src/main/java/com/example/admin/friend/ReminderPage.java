package com.example.admin.friend;

import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TimePicker;
import android.widget.Toast;

import java.util.Calendar;


public class ReminderPage extends android.support.v4.app.Fragment {
    View view;
Button Setalarm;
    TimePicker timePicker;
    final static int RQS_1=1;
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        view= inflater.inflate(R.layout.fragment_reminder, container, false);
        timePicker=(TimePicker)view.findViewById(R.id.timePicker);
        Setalarm=(Button)view.findViewById(R.id.button8);
        Calendar calendar=Calendar.getInstance();
        timePicker.setCurrentHour(calendar.get(Calendar.HOUR_OF_DAY));
        timePicker.setCurrentMinute(calendar.get(Calendar.MINUTE));
        Setalarm.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Calendar current = Calendar.getInstance();
                Calendar cal = Calendar.getInstance();
                cal.set(timePicker.getCurrentHour(), timePicker.getCurrentMinute(), 00);
                if (cal.compareTo(current) <= 0) {
                    Toast.makeText(getContext(), "Invalid time", Toast.LENGTH_LONG).show();
                } else {
                    setAlarm(cal);
                }

            }

            private void setAlarm(Calendar targetcal) {
                Toast.makeText(getContext(),"Alarm is set",Toast.LENGTH_LONG).show();
                Intent intent=new Intent(getContext(),AlarmReceiver.class);
                PendingIntent pendingIntent=PendingIntent.getBroadcast(getContext(),RQS_1,intent,0);
                AlarmManager alarmmanager=(AlarmManager)getActivity().getSystemService(Context.ALARM_SERVICE);
                alarmmanager.set(AlarmManager.RTC_WAKEUP,targetcal.getTimeInMillis(),pendingIntent);
            }
        });


        return view;
    }
}
