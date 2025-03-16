# PACHI: Power Apps x Canvas Headless Integration
Ever wished Copilot could interact with your Power Apps for you? **Look no further!**

I built PACHI to demonstrate how Copilot could open and interact with a Power Apps canvas app on a user's behalf to satisfy their request within a Copilot chat session.

## How PACHI Works
PACHI uses a few uninque capabilities strung together into one cohesive system:

Within a chat session with Copilot (in this demo, just a generic agent, but pretent it is Copilot!), these are the steps that happen:

### Step 1: The User Requests Copilot Use a Power App to Accomplish Something
Much like how a user can tag a teams channel or other resources within Copilot, the user could in theory tag a Power App they have access to.

In the example above, the user is requesting that Copilot **use the "Contact Entry" app to save a new contact with first name 'Tim', last name 'Hanewich', email 'timhan@gmail.com', and phone number '9417774444'**.

### Step 2: Download App Schema
Upon this being requested, a tool is called. This tool kicks off a multi-step process.

Firstly, the canvas app is downloaded. The PAC CLI is used to download the `.msapp` file. After it is downloaded, the PAC CLI is then again used to unpack the .msapp into a working directory.

### Step 3: Load App Schema Into Headless Simulator
With the canvas app schema downloaded, it is now loaded as a [CanvasApp](./src/PowerAppsComponents/CanvasApp.cs) object and then subsequently loaded into a [CanvasSimulator](./src/Simulation/CanvasSimulator.cs) object, a class designed to simulate interacting with a canvas app (clicking + typing) in a headless, pure-text environmnt.

### Step 4: Describe to the LLM what is on the screen and ask it what to do
With the canvas app ready to be simulated, we now describe to the LLM what it is seeing on the first screen.

For the current screen the headless simulator is on, the `Describe()` function is used to produce a human-readable string that describes what is visually present on the screen. The **task** that the LLM is being charged with and a **question of what to do next** is appended at the bottom.

```
You are using an app named 'Contact Entry' and you are on a screen named 'WelcomeScreen' within the app.

This is what you see on the screen:
- A label named 'WelcomeTitle' that says 'Welcome to the app where you can enter in new Contact records!'
- A label named 'ClickBelowInstructions' that says 'Click the enter button below to start entering a new contact record'
- A button named 'EnterButton' that says 'Enter'

Your task is: Add contact record with name 'Tim Hanewich', email 'timhanewich@email.com', and phone number 9417774321.

What do you do next?
```

### Step 5: The LLM Decides what to do
As specified in [the system prompt](./src/prompts/system.md), the model can respond in one of three ways:

By clicking a control, which would look like this:
```
{
    "action": "click",
    "control": "SubmitFormButton"
}
```

Or by typing text into an input box, which would look like this:

```
{
    "action": "type",
    "control": "LastNameInput",
    "text": "Smith"
}
```

Or, if the model thinks the task it was charged with is now complete, indicating that like this:

```
{
    "action": "complete"
}
```

**JSON mode** is used to force the model to respond as a JSON object that can be deserialized.

### Step 6: We Simulate that Decision
The LLM's decision on what is best to do in this situation is simulated using the `CanvasSimulator` described in step 3. 

Whether it is clicking a button or entering in text, the accompanying Power Fx is read and executed; for example, if a button's `OnSelect` property is using a `Navigate` function to navigate to another screen, this is simulated.

In the example above, let's assume the LLM decided to click on the button "EnterButton".

### Step 7: We, again, ask the LLM what to do
After executing the last decision it made, we now again desribe to the LLM what the result looks like (i.e. a new screen may have been navigated to where new controls are now being presented).

For example, after clicking "EnterButton" in the previous example, that would have navigated to another screen in which this is now the situation being described to the LLM:

```
You are using an app named 'Contact Entry' and you are on a screen named 'ContactDetailsScreen' within the app.

This is what you see on the screen:
- A form named 'ContactForm' mapped to the 'Contacts' data source with the following fields (text input boxes):
        - Text input box named 'DataCardValue8' mapped to the data field 'mobilephone', currently containing ""
        - Text input box named 'DataCardValue1' mapped to the data field 'firstname', currently containing ""
        - Text input box named 'DataCardValue2' mapped to the data field 'lastname', currently containing ""
        - Text input box named 'DataCardValue3' mapped to the data field 'emailaddress1', currently containing ""
- A button named 'SubmitFormButton' that says 'Submit Record'

So far you have:
- clicked a button control named 'EnterButton'

Your task is: Add contact record with name 'Tim Hanewich', email 'timhanewich@email.com', and phone number 9417774321.
```

Above you'll notice that after the first decison is made, we then append a continuous list of **every decision** the model is making and that we execute. This allows the model to have context as to what it has already done which is very important for it being able to know it has completed its task.

### Step 8: Repeat Steps 4-7 Until the Task is Complete
We repeat steps 4-7 until the LLM finally indicates the task that it was assigned to do is complete using the `action: complete` output.

After doing this a few times, this is an example of what the prompt to the model may look like:

```
You are using an app named 'Contact Entry' and you are on a screen named 'WelcomeScreen' within the app.

This is what you see on the screen:
- A label named 'WelcomeTitle' that says 'Welcome to the app where you can enter in new Contact records!'
- A label named 'ClickBelowInstructions' that says 'Click the enter button below to start entering a new contact record'
- A button named 'EnterButton' that says 'Enter'

So far you have:
- clicked a button control named 'EnterButton'
- Typed 'Tim' into a text box named 'DataCardValue1'
- Typed 'Hanewich' into a text box named 'DataCardValue2'
- Typed 'timhanewich@email.com' into a text box named 'DataCardValue3'
- Typed '9417774321' into a text box named 'DataCardValue8'
- clicked a button control named 'SubmitFormButton'
- clicked a button control named 'GoBackToWelcomeScreenButton'

Your task is: Add contact record with name 'Tim Hanewich', email 'timhanewich@email.com', and phone number 9417774321.
```

As you can see above, the model has gone through all the steps necessary... clicking the enter button, entering in details into their appropriate fields on the form, clicking submit button (which will execute PowerFX code to make the Dataverse POST), and then stepping through the success screen.

At this point, upon prompting *this* to the model, the model responds with an `action: complete` output to confirm that the task it was assigned to do is now complete.