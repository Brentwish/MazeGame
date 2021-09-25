function loopErasedRandomWalk() {
  var i1;

  // Pick a location that’s not yet in the maze (if any).
  if (i0 == null) {
    do if ((i0 = remaining.pop()) == null) return true;
    while (cells[i0] >= 0);
    previous[i0] = i0;
    fillCell(i0);
    x0 = i0 % cellWidth;
    y0 = i0 / cellWidth | 0;
    return;
  }

  // Perform a random walk starting at this location,
  // by picking a legal random direction.
  while (true) {
    i1 = Math.random() * 4 | 0;
    if (i1 === 0) { if (y0 <= 0) continue; --y0, i1 = i0 - cellWidth; }
    else if (i1 === 1) { if (y0 >= cellHeight - 1) continue; ++y0, i1 = i0 + cellWidth; }
    else if (i1 === 2) { if (x0 <= 0) continue; --x0, i1 = i0 - 1; }
    else { if (x0 >= cellWidth - 1) continue; ++x0, i1 = i0 + 1; }
    break;
  }

  // If this new cell was visited previously during this walk,
  // erase the loop, rewinding the path to its earlier state.
  if (previous[i1] >= 0) eraseWalk(i0, i1);

  // Otherwise, just add it to the walk.
  else {
    previous[i1] = i0;
    fillCell(i1);
    if (i1 === i0 - 1) fillEast(i1);
    else if (i1 === i0 + 1) fillEast(i0);
    else if (i1 === i0 - cellWidth) fillSouth(i1);
    else fillSouth(i0);
  }

  // If this cell is part of the maze, we’re done walking.
  if (cells[i1] >= 0) {

    // Add the random walk to the maze by backtracking to the starting cell.
    // Also erase this walk’s history to not interfere with subsequent walks.
    context.save();
    context.fillStyle = "#fff";
    fillCell(i1);
    while ((i0 = previous[i1]) !== i1) {
      fillCell(i0);
      if (i1 === i0 + 1) cells[i0] |= E, cells[i1] |= W, fillEast(i0);
      else if (i1 === i0 - 1) cells[i0] |= W, cells[i1] |= E, fillEast(i1);
      else if (i1 === i0 + cellWidth) cells[i0] |= S, cells[i1] |= N, fillSouth(i0);
      else cells[i0] |= N, cells[i1] |= S, fillSouth(i1);
      previous[i1] = NaN;
      i1 = i0;
    }
    context.restore();

    previous[i1] = NaN;
    i0 = null;
  } else {
    i0 = i1;
  }
}

function eraseWalk(i0, i2) {
  var i1;
  context.save();
  context.globalCompositeOperation = "destination-out";
  do {
    i1 = previous[i0];
    if (i1 === i0 - 1) fillEast(i1);
    else if (i1 === i0 + 1) fillEast(i0);
    else if (i1 === i0 - cellWidth) fillSouth(i1);
    else fillSouth(i0);
    fillCell(i0);
    previous[i0] = NaN;
    i0 = i1;
  } while (i1 !== i2);
  context.restore();
}

function fillCell(i) {
  var x = i % cellWidth, y = i / cellWidth | 0;
  context.fillRect(x * cellSize + (x + 1) * cellSpacing, y * cellSize + (y + 1) * cellSpacing, cellSize, cellSize);
}

function fillEast(i) {
  var x = i % cellWidth, y = i / cellWidth | 0;
  context.fillRect((x + 1) * (cellSize + cellSpacing), y * cellSize + (y + 1) * cellSpacing, cellSpacing, cellSize);
}

function fillSouth(i) {
  var x = i % cellWidth, y = i / cellWidth | 0;
  context.fillRect(x * cellSize + (x + 1) * cellSpacing, (y + 1) * (cellSize + cellSpacing), cellSize, cellSpacing);
}