const m3units = "m\u00b3";

function createRecipes(obj) {
  const recipes = {
    standard: [],
    alternate: [],
    handBuilt: []
  };
  obj.map(x => {
    const recipe = new Recipe(x);
    if (recipe.isAlternate) {
      recipes.alternate.push(recipe);
    } else {
      recipes.standard.push(recipe);
    }
  });

  return recipes;
}

class Part {
  constructor(name, building, rate, units, inputs, multiplier, depth) {
    this.key = getPartKey(name);
    this.name = name;
    this.building = building;
    this.rate = rate;
    this.inputs = inputs;
    this.multiplier = multiplier || 1;
    this.depth = depth || 0;
    this.units = units || "";
  }

  static hydrate(obj) {
    const inputs = obj.inputs.map(x => InputRate.create(x));
    return new Part(obj.name, obj.building, obj.rate, obj.units, inputs, obj.multiplier, obj.depth);
  }


}

class InputRate {
  constructor(part, rate) {
    this.part = part;
    this.rate = rate;
  }
}

class BillOfMaterials {
  constructor(node) {
    this.parts = [];
    this.add(node);
  }

  add(node) {
    const existing = this.find(node.value.name);
    if (existing) {
      existing.rate += node.value.rate;
      existing.numBuildings += node.value.numBuildings;
    } else {
      this.parts.unshift(node.value);
    }
    node.children.map(x => this.add(x));
    return this.sort();
  }

  find(name) {
    const p = this.parts;
    for (var i = 0; i < p.length; i++) {
      if (p[i].name === name) {
        return p;
      }
    }
    return null;
  }

  sort() {
    return this.parts.sort((a, b) => {
      let cmp = a.depth - b.depth;
      if (cmp === 0) {
        return a.name < b.name ? -1 : a.name > b.name ? 1 : 0;
      }
      return cmp;
    });
  }

  log() {
    this.parts.map(x => x.log());
  }
}

class SolveResult {
  constructor(name, rate, units, building, numBuildings, depth) {
    this.name = name;
    this.rate = rate;
    this.units = units || "";
    this.numBuildings = numBuildings;
    this.building = building;
    this.depth = depth || 0;
  }

  log(useIndent) {
    let indent = "";
    if (useIndent)
      for (let i = 0; i < this.depth; i++)
        indent += "  ";

    console.log(
      `${indent}${this.name} @ ${this.rate.toFixed(2)}${this.units}/min, ${this.numBuildings.toFixed(2)} ${this.building
      }`);
  }
}

class SolveNode {
  constructor(value) {
    this.value = value;
    this.children = [];
  }

  log() {
    try {
      console.log("Solve tree:");
      console.log("-----------");
      this.value.log(true);

      console.log();
      console.log("Full BOM:");
      console.log("---------");
      const bom = this.getBom();
      bom.log();
    } catch (error) {
      console.error(error);
    }
  }

  // bill of materials
  getBom() {
    return new BillOfMaterials(this);
  }
}

function findInputs(parts, recipe) {
  const inputs = [];
  const missing = [];
  if (!recipe.isMined) {
    for (var i = 0; i < recipe.inputs.length; i++) {
      const recipeInput = recipe.inputs[i];

      const key = getPartKey(recipeInput.name);
      const input = parts[key];
      if (!input) {
        missing.push(recipeInput.name);
      } else {
        const rate = recipeInput.rate;
        inputs.push(new InputRate(input, rate));
      }
    }
  }
  return {
    inputs: inputs,
    missing: { name: recipe.name, inputs: missing }
  };
}

function removeMatchingRecipes(recipes, match) {
  return recipes.filter(r => match.filter(m => !m.outputs || m.outputs.filter(o => o.name === r.name).length));
}

function init(data, unlocked) {
  let allRecipes = createRecipes(data);
  let recipes = allRecipes.standard;
  let alternates = allRecipes.alternate;

  if (unlocked) {
    const match = alternates.filter(x => unlocked.includes(x.name));
    recipes = removeMatchingRecipes(recipes, unlocked).concat(match);
  }

  let parts = {};

  let skipped = [];
  while (recipes.length > 0 && skipped.length < recipes.length) {
    const p = recipes.shift();

    if (!p) {
      debugger;
    }

    let inputs = [];
    let gotInputs = true;

    if (!p.isMined) {
      const foundInputs = findInputs(parts, p);
      if (foundInputs.missing.length) {
        recipes.push(p);
        gotInputs = false;
        skipped.push(p);
        continue;
      } else {
        inputs = foundInputs.inputs;
      }
    }
    if (gotInputs) {
      var part = new Part(p.name, p.building, p.rate, p.units, inputs);
      if (!inputs) {
        throw `missing inputs for ${p.name}`;
      }
      parts[part.key] = part;
      skipped = [];
    }
  }

  if (skipped.length > 0) {
    const missing = skipped.map(x => findInputs(parts, x).missing).filter(x => x.length > 0);
    console.warn("Invalid data! The following parts had missing inputs:");
    console.warn(skipped);

    console.warn("Missing inputs:");
    console.warn(missing);
  }

  return { parts: parts, alternates: alternates };
}

function getPartKey(name) {
  return name.toLowerCase().replace(/[^a-z0-9]/g, "");
}

function lookupPart(parts, partial) {
  const partialKey = getPartKey(partial);
  if (parts[partialKey]) {
    return parts[partialKey];
  }
  for (var key in parts) {
    if (key.startsWith(partialKey)) {
      return parts[key];
    }
  }
}


function onPromptError(err) {
  console.warn(err);
  return 1;
}

try {
  if (process === undefined) {
  } else {
    const got = require("got");

    (async () => {
      try {
        const response = await got("https://satisfactory.z5.web.core.windows.net/recipes.json").json();
        const z = init(response);
        const parts = z.parts;
        const alternates = z.alternates;

        const myArgs = process.argv.slice(2);
        let solvePartName = myArgs.length > 0 ? myArgs[0] : null;
        let solveRate = myArgs.length > 1 ? parseFloat(myArgs[1]) : null;

        const part = lookupPart(parts, solvePartName);
        const node = part.solve(solveRate);
        node.log();
      } catch (error) {
        console.error(error);
      }
    })();

  }
} catch (error) {
  console.warn(error);
}