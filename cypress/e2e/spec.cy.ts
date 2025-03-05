describe('template spec', () => {
  it('passes', () => {
    cy.visit('http://localhost:5173/')
    cy.get("button").click()
    cy.get("button").should("contain", "count is 1")
  })
})