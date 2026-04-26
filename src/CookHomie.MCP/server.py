from fastmcp import FastMCP

from tools.spike import hello_world


def create_mcp() -> FastMCP:
    app = FastMCP("CookHomie MCP")
    app.tool()(hello_world)
    return app


mcp = create_mcp()


if __name__ == "__main__":
    mcp.run()
