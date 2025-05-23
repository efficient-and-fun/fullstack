import dayjs from "dayjs";
import "dayjs/locale/de";
dayjs.locale("de");

const time = dayjs().valueOf();
const baseURL = "http://localhost:5173"

var creds = {
  username: `name${time}`,
  email: `name${time}@exmaple.com`,
  pwd1: "Abcdefg123!",
  pwd2: "Abcdefg123!",
  path: "C:/folder/images",
  isAGBAccepted: true,
};

describe("Authentication", () => {
  it("should register a random user and login for further tests", () => {
    cy.visit(`${baseURL}/register`);
    cy.get(".cy-register-username").type(`${creds.username}`);
    cy.get(".cy-register-email").type(`${creds.email}`);
    cy.get(".cy-register-pwd1").type(`${creds.pwd1}`);
    cy.get(".cy-register-pwd2").type(`${creds.pwd2}`);
    cy.get(".cy-register-submitbutton").click();
    cy.contains("Home"); //Startseite sollte geladen werden können.
  });
});

describe("Navigation", () => {
  //Setup für einzelne Tests.
  beforeEach(() => {
    cy.session(
      "login",
      () => {
        cy.visit(`${baseURL}/login`);

        cy.get('.cy-login-email').type(`${creds.email}`);

        cy.get('.cy-login-pwd').type(`${creds.pwd1}`);
        cy.get('.cy-login-loginbutton').click();
        cy.window().its("localStorage.authToken").should("be.a", "string");
      });
  });


  it("should navigate to Event page when clicking the Event link", () => {
    cy.visit(baseURL);
    cy.contains("Event").click(); // Link mit Text "Event" anklicken
    cy.url().should("include", "/event"); // URL sollte sich ändern
    cy.contains("Event Page"); // Der Inhalt der Event-Seite sollte angezeigt werden.
  });
  it("should navigate to Home page when clicking the Home link", () => {
    cy.visit(baseURL);
    cy.contains("Home").click(); // Link mit Text "Home" anklicken.
    cy.url().should("include", "/"); // URL sollte sich ändern.
    cy.get(".MuiTypography-h6").contains(dayjs().format("DD. MMMM YYYY"));
  });
  it("should navigate to Event page when clicking the Notification link", () => {
    cy.visit(baseURL);
    cy.contains("Notification").click(); // Link mit Text "Notification" anklicken.
    cy.url().should("include", "/notification"); // URL sollte sich ändern.
    cy.contains("Notification Page"); // Der Inhalt der Event-Seite sollte angezeigt werden.
  });
  it("should navigate to Friends page when clicking the Friends link", () => {
    cy.visit(baseURL);
    cy.contains("Friends").click(); // Link mit Text "Friends" anklicken
    cy.url().should("include", "/friends"); // URL sollte sich ändern
    cy.contains("My Friends"); // Der Inhalt der Friends-Seite sollte angezeigt werden.
  });
  it("should navigate to Setting page when clicking the Setting link", () => {
    cy.visit(baseURL);
    cy.contains("Setting").click(); // Link mit Text "Setting" anklicken
    cy.url().should("include", "/setting"); // URL sollte sich ändern
    cy.contains("Setting Page"); // Der Inhalt der Setting-Seite sollte angezeigt werden.
  });
}); 