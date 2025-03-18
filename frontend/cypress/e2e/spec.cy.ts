describe("template spec", () => {
  it("passes", () => {
    cy.visit("http://localhost:5173/");
    cy.get("button").first().click();
    cy.get("button").should("contain", "count is 1");
    cy.get("h1").should("contain", "Vite + React");
  });
});
