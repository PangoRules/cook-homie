from fastmcp import FastMCP

from tools.spike import hello_world

mcp = FastMCP("CookHomie MCP")
mcp.tool()(hello_world)


if __name__ == "__main__":
    mcp.run()
