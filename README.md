# Gardening Calendar
A simple WPF application to generate planting dates for your region based on frost dates.

## Summary
I wrote this so I could easily generate planting date ranges for my area, and have a little flexibility. All dates for the supported plants are stored in the config file, and can be extended for your region or tweaked depending on your experiences.

## How To Use
First, I strongly recommend you create a new Google Calendar specifically for these records. It's going to generate a ton of entries, and you likely don't want them mucking up your day to day calendar. Plus it's not smart enough to not delete your normal events, so if you regenerate for any reason it will wipe out your existing Calendar.

Once you've created that, go ahead and run the application. It will prompt you to log into your Google account, and grant access to your Calendars. After that, it should be pretty straight forward. Just select the calendar you made, enter your first and last frost dates (which can be pulled from any number of sites), and click Save. You'll be able to see a preview of the dates before you commit.

## What needs fixing
- Better protections against bad data entry
- Better headers for the output grid
- Some way to allow this to add to existing calendars and only remove records it added
