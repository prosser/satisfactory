using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatisfactoryTools.wwwroot
{
    public class SolverViewModel
    {
        let savedRates = { };
        let selectedPart = null;
        let parts = { };
        let alternates = [];
        let unlocked = [];
        let data = null;

      
        
        function onDataLoaded()
        {
            const z = init(data, unlocked);
            parts = z.parts;
            alternates = z.alternates;

            document.getElementById("parts").innerHTML = getPartsListHtml(parts);
            document.getElementById('alternates').innerHTML = getAlternatesHtml();

            if (selectedPart)
            {
                const elem = document.getElementById("part");
                const options = elem.options;
                for (let i = 0; i < options.length; i++)
                {
                    if (options[i].value === selectedPart)
                    {
                        elem.selectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                selectPart();
            }
            solve();
        }

        function getResultHtml(result, depth)
        {
            return `< div style = "margin-left: ${depth * 10}px" >` +
                `< div class="part">${result.name
    }</div>` +
                `<div class="rate">@ ${result.rate.toFixed(2)}${result.units}/min</div> with ` +
                `<div class="numBuildings">${result.numBuildings.toFixed(2)}</div>` +
                `<div class="building">${result.building}</div>` +
                '</div>'
        }

        function getPartsListHtml(parts)
{
    let partNames = [];

            for (var key in parts)
    {
        partNames.unshift(parts[key].name);
    }
    partNames = partNames.sort();

    return '<select id="part" onchange="selectPart()">' + partNames.map(x => {
        const key = getPartKey(x);
        return `< option value = "${key}" >${ x}</ option >`;
    }).join("\n") + '</select>';
}

function getAlternatesHtml()
{
    return alternates.map(x => {
    return `< div class="altRecipe"><input type = "checkbox" onclick="unlockAlts()" ${isUnlocked(x) ? "checked " : ""}data-part="${x.name}" /> ${x.name}</div>`;
            }).join("\n");
        }

        function isUnlocked(part)
{
    const result = unlocked.filter(x => x === part.name).length > 0;
    return result;
}

function getTreeHtml(node, depth)
{
    depth = depth || 0;
    let html = getResultHtml(node.value, depth);

    node.children.map(x => {
        html += getTreeHtml(x, depth + 1);
    });

    return html;
}

function unlockAlts()
{
    unlocked = [];
    var inputs = document.getElementById('alternates').getElementsByTagName("input");
    for (let i = 0; i < inputs.length; i++)
    {
        if (inputs[i].checked) {
        unlocked.push(inputs[i].getAttribute("data-part"));
    }
}
writeLocalStorage("unlocked", unlocked);
onDataLoaded();
        }

        function getBomHtml(node)
{
    const lines = new BillOfMaterials(node).parts.map(x => getResultHtml(x, 0));
    return lines.join("");
}

function getNodeHtml(node)
{
    return '<div class="tree"><h2>Hierarchy</h2>' +
        getTreeHtml(node, 0) +
        '</div>' +
        '<div class="bom"><h2>BOM</h2>' +
        getBomHtml(node) +
        '</div>';
}

function getRate()
{
    return document.getElementById('rate').value;
}

function setRate(rate)
{
    document.getElementById('rate').value = rate;
    writeLocalStorage("rate", rate);
}

function selectPart()
{
    const elem = document.getElementById('part');
    selectedPart = elem.value;
    writeLocalStorage("selectedPart", selectedPart);
    solve();
}

function solve()
{
    if (!selectedPart)
    {
        return;
    }
    const rate = document.getElementById('rate').value;
    setRate(rate);
    const node = parts[selectedPart].solve(parseFloat(rate));
    document.getElementById("results").innerHTML = getNodeHtml(node);
}
    }
}
