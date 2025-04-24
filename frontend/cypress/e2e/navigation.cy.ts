import dayjs from "dayjs";
describe('Navigation', () => {
    it('should navigate to Event page when clicking the Event link', () => {
      cy.visit('http://localhost:5173'); // Deine Dev-URL
  
      cy.contains('Event').click(); // Link mit Text "Event" anklicken
  
      cy.url().should('include', '/event'); // URL sollte sich ändern
      cy.contains('Event Page'); // Der Inhalt der Event-Seite sollte angezeigt werden
    });

    it('should navigate to Home page when clicking the Home link', () => {
        cy.visit('http://localhost:5173'); // Deine Dev-URL
    
        cy.contains('Home').click(); // Link mit Text "Home" anklicken
    
        cy.url().should('include', '/'); // URL sollte sich ändern
        cy.get(".MuiTypography-h6").contains(dayjs().format("DD. MMMM YYYY"));
      });

    it('should navigate to Event page when clicking the Notification link', () => {
        cy.visit('http://localhost:5173'); // Deine Dev-URL

        cy.contains('Notification').click(); // Link mit Text "Notification" anklicken

        cy.url().should('include', '/notification'); // URL sollte sich ändern
        cy.contains('Notification Page'); // Der Inhalt der Event-Seite sollte angezeigt werden
    });

    it('should navigate to Friends page when clicking the Friends link', () => {
        cy.visit('http://localhost:5173'); // Deine Dev-URL
    
        cy.contains('Friends').click(); // Link mit Text "Friends" anklicken
    
        cy.url().should('include', '/friends'); // URL sollte sich ändern
        cy.contains('Friend Page'); // Der Inhalt der Friends-Seite sollte angezeigt werden
    });

    it('should navigate to Setting page when clicking the Setting link', () => {
        cy.visit('http://localhost:5173'); // Deine Dev-URL

        cy.contains('Setting').click(); // Link mit Text "Setting" anklicken

        cy.url().should('include', '/setting'); // URL sollte sich ändern
        cy.contains('Setting Page'); // Der Inhalt der Setting-Seite sollte angezeigt werden
    });
  });