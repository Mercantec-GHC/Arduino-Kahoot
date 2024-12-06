#include <WiFiNINA.h>
#include <Arduino_JSON.h>
#include <Arduino_MKRIoTCarrier.h>

// WiFi credentials
const char* ssid = "YourWiFiSSID";
const char* password = "YourWiFiPassword";

// Backend API
const char* server = "https://arduino-kahoot.onrender.com"; // Replace with your server address
const int port = 80;
String quizEndpoint = "/api/Quiz/";   // Endpoint for quiz data
String answerEndpoint = "/api/Quiz/Answer"; // Endpoint to submit answers

// Quiz data
int correctAnswerIndex = -1; // Correct answer index

// MKR IoT Carrier object
MKRIoTCarrier carrier;

// WiFi client
WiFiClient client;

// Selected answer
int selectedAnswer = -1; // No selection initially

void setup() {
  Serial.begin(9600);
  carrier.begin();

  // Turn off OLED initially
  carrier.display.fillScreen(ST77XX_BLACK);
  carrier.display.display();

  // Connect to WiFi
  connectToWiFi();

  // Wait for the frontend to provide a quiz ID
  waitForQuizSelection();
}

void loop() {
  // Check for button presses
  checkButtons();

  // Add delay to avoid rapid input
  delay(200);
}

void connectToWiFi() {
  Serial.print("Connecting to WiFi");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("\nConnected to WiFi!");
}

void waitForQuizSelection() {
  Serial.println("Waiting for quiz selection from frontend...");

  // Example: Replace with actual method to wait for quiz ID from frontend
  int quizId = 1; // Placeholder for selected quiz ID
  fetchQuestion(quizId);
}

void fetchQuestion(int quizId) {
  String endpoint = quizEndpoint + String(quizId) + "/Next";

  if (client.connect(server, port)) {
    client.println(String("GET ") + endpoint + " HTTP/1.1");
    client.println(String("Host: ") + server);
    client.println("Connection: close");
    client.println();

    // Read the response
    String response = "";
    while (client.connected() || client.available()) {
      response += client.readString();
    }

    // Parse the response
    parseQuestion(response);
  } else {
    Serial.println("Failed to connect to server.");
  }
}

void parseQuestion(String response) {
  // Extract JSON payload
  int startIndex = response.indexOf("\r\n\r\n") + 4;
  String json = response.substring(startIndex);

  JSONVar questionData = JSON.parse(json);
  if (JSON.typeof(questionData) == "undefined") {
    Serial.println("Failed to parse JSON.");
    return;
  }

  for (int i = 0; i < questionData["answers"].length(); i++) {
    if ((bool)questionData["answers"][i]["isCorrect"]) {
      correctAnswerIndex = i;
    }
  }
}

void checkButtons() {
  carrier.Buttons.update();

  // Map buttons to answer selections
  if (carrier.Buttons.getButton(0)->isPressed()) {
    selectedAnswer = 0;
    submitAnswer();
  } else if (carrier.Buttons.getButton(1)->isPressed()) {
    selectedAnswer = 1;
    submitAnswer();
  } else if (carrier.Buttons.getButton(2)->isPressed()) {
    selectedAnswer = 2;
    submitAnswer();
  } else if (carrier.Buttons.getButton(3)->isPressed()) {
    selectedAnswer = 3;
    submitAnswer();
  }
}

void submitAnswer() {
  if (selectedAnswer == -1) return;

  if (client.connect(server, port)) {
    // Send POST request with the selected answer
    client.println(String("POST ") + answerEndpoint + " HTTP/1.1");
    client.println(String("Host: ") + server);
    client.println("Content-Type: application/json");
    client.println("Connection: close");

    // JSON payload
    String payload = "{";
    payload += "\"selectedAnswer\": " + String(selectedAnswer);
    payload += "}";
    client.println("Content-Length: " + String(payload.length()));
    client.println();
    client.println(payload);

    // Read the response
    String response = "";
    while (client.connected() || client.available()) {
      response += client.readString();
    }

    // Check correctness and update OLED
    checkCorrectness(response);

    // Pause before turning off the display
    delay(2000);

    // Turn off OLED
    carrier.display.fillScreen(ST77XX_BLACK);
    carrier.display.display();
  } else {
    Serial.println("Failed to submit answer.");
  }
}

void checkCorrectness(String response) {
  // Parse JSON response
  int startIndex = response.indexOf("\r\n\r\n") + 4;
  String json = response.substring(startIndex);

  JSONVar result = JSON.parse(json);
  if ((bool)result["isCorrect"]) {
    Serial.println("Correct Answer!");
    carrier.display.fillScreen(ST77XX_GREEN); // Green screen for correct answer
  } else {
    Serial.println("Wrong Answer.");
    carrier.display.fillScreen(ST77XX_RED); // Red screen for wrong answer
  }

  carrier.display.display();
}