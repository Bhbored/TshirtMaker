---
name: t-shirtwebsite
description: This is a new rule
---

# Overview
1- always chose the simplest way to resolve any proble.
2- treat this project as for an expert c# engineer but not so much frontend 
3 - alwasy use the simplest way of styling to achieve the best result
4 - use boostrap and tailwind (when needed tailwind), as much as possible to lessen the use of hardcoded css.
5 - in app.css just add the main fonts and weight needed for titles and body of ur choice from google fonts (add them to app.razor) and adjsut the app.css
6- create themes.css each component (or selecter) should have a --cutom-btn-core , which has by default in the :root the --custom-btn-light ,
every ui component in color aspect should have light and dark variation , use the [data-custom-light] attribute selector and [data-custom-dark] to switch the core varible into light or dark
7- for layouts alwasy use boostrap and it's components and override them to look modern
8- themes.css hold the colors aspect , and use isolated css of the Pages as much as possible to catch errors.
9 - just a simple js method to switch between light and dark no need for saving the theme reference in the browser db !!
10 - use clean architecture components/Pages/subPages (if any ) -- ui/(common - Page1Name - etc..) , emptylayout for the login and sign up
11 - use moden futuristic colors of ur choice , with minimal animations like fade in fade out ease in etc.. for the Main ui block of the Pages 
12 - this project will use supabse for db and authentication , dothe services for it but don't use them in the ui or login flow , just make them read for me , create Testdata for the models and inject TesData and use them with one unified service like product user etc...
13 - for the ai parts make seperate folders for it next to the model, it should Take dto's intent repsonse and the service class in the services folder , for using the api key there's 2 approachs one with http request and one with the openai package pick the simplest one and apply it 
14 - no comments unless it's necessary or it's the ai service class i want to learn that 
15 - when ur finsihed cerate a read me file about this project summery and teh supabase setup step by step and bonus if u gave the ai service explination


