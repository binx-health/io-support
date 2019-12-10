% Load necessary packages
pkg load signal

%------------------------------------------------------------------
%                               Functions
%------------------------------------------------------------------
function mp1_writesignal(outputFile, name, V, C)
    fprintf(outputFile, 'Signal,%s\r\n%s\r\n%s\r\n', name, strjoin(cellstr(num2str(V(:), '%.20g')), ','), strjoin(cellstr(num2str(C(:), '%.20g')), ','));
end


function a = mp1_polyfit(x, y, n)
    nans = find(isnan(y));
    x(nans) = [];
    y(nans) = [];
    a = polyfit(x, y, n);
end


function [sigma_out, mu_out, alpha_out, h_out] = mp1_inc_alpha(sigma_in, mu_in, alpha_in, h_in, delta)
    sigma_out = sigma_in;
    mu_out = mu_in + mp1_mss(sigma_in, alpha_in) - mp1_mss(sigma_in, alpha_in + delta);
    alpha_out = alpha_in + delta;
    h_out = h_in;
end


function [sigma_out, mu_out, alpha_out, h_out] = mp1_inc_h(sigma_in, mu_in, alpha_in, h_in, delta)
    sigma_out = sigma_in;
    mu_out = mu_in;
    alpha_out = alpha_in;
    h_out = max(h_in + delta, 0);
end


function [sigma_out, mu_out, alpha_out, h_out] = mp1_inc_mu(sigma_in, mu_in, alpha_in, h_in, delta)
    sigma_out = sigma_in;
    mu_out = mu_in + delta;
    alpha_out = alpha_in;
    h_out = h_in;
end


function [sigma_out, mu_out, alpha_out, h_out] = mp1_inc_sigma(sigma_in, mu_in, alpha_in, h_in, delta)
    sigma_out = max(sigma_in + delta, 0);
    mu_out = mu_in;
    alpha_out = alpha_in;
    h_out = h_in;
end


function result = mp1_var(Gtfc, sigma, mu, alpha, h, V, P)
    % Optimised implementation
    alphaOverSqrt2Sigma = alpha / (sqrt(2) * sigma);
    oneOver2SigmaSquared = 1 / (2 * sigma^2);
    oneOverSigmaSqrt2pi = 1 / (sigma * sqrt(2 * pi));
    toErf = (V .- mu) .* alphaOverSqrt2Sigma;
    factor = (1 ./ (abs(toErf) .* 0.3275911 .+ 1));
    erfVal = (((((factor .* 1.061405429) .+ -1.453152027) .* factor .+ 1.421413741) .* factor .+ -0.284496736) .* factor .+ 0.254829592) .* factor;
    erfVal = sign(toErf) .* (1 .- erfVal .* exp(-toErf .* toErf));
    sgv = (1 .+ erfVal) .* exp(-(V .- mu).^2 .* oneOver2SigmaSquared) .* oneOverSigmaSqrt2pi;
    msgv = max(sgv);
    use = find((sgv >= (1 - Gtfc) * msgv) & !isnan(P));
    hOverMsgv = h / msgv;
    result = sum((P(use) .- hOverMsgv .* sgv(use)).^2) / size(use)(2);
end


function result = mp1_erf(x)
    factor = (1 / (1 + 0.3275911 * abs(x)));
    result = (((((1.061405429 * factor) + -1.453152027) * factor + 1.421413741) * factor + -0.284496736) * factor + 0.254829592) * factor;
    result = sign(x) * (1 - result * exp(-x * x));
end


function result = mp1_sgv(sigma, mu, alpha, x)
    result = (1 + mp1_erf(alpha * (x - mu) / (sqrt(2) * sigma))) * exp(-(x - mu)^2 / (2 * sigma^2)) / (sigma * sqrt(2 * pi)); 
end


function result = mp1_mss(sigma, alpha)
    result = sqrt(2 / (pi * (1 + alpha^2))) * sigma * alpha;
end


function result = mp1_sqp(sigma, mu, alpha)
    result = mu + mp1_mss(sigma, alpha);
    delta = sigma;
    converged = false;
    while !converged
        current = mp1_sgv(sigma, mu, alpha, result);
        pos = mp1_sgv(sigma, mu, alpha, result + delta);
        neg = mp1_sgv(sigma, mu, alpha, result - delta);
        if pos > neg && pos > current
            result = result + delta;
        elseif neg > pos && neg > current
            result = result - delta;
        end
        if (delta >= 0.01)
            delta = delta / 2;
        else
            converged = true;
        end
    end
end


function [Pass, F] = mp1_gaussfit(outputFile, G, V, P)
    
    fprintf(outputFile, 'Log,FitGaussianForPeak(...)\r\n');
    
    Pass = true;
    Present = false;
    F = struct('oc', 'False', 'sigma', G.wu / 4, 'mu', G.mu, 'h', 0, 'alpha', 0, 'p', G.mu, 'var', 0.0);
    
    % 1. Check variance
    if Pass
        varValue = 0.0;
        varCount = 0;
        for n = 1:size(P)(2)
            if !isnan(P(n)) && V(n) >= (G.rl - 3) && V(n) <= G.ru
                varValue += P(n)^2;
                ++varCount;
            end
        end
        varValue = varValue / varCount;
        fprintf(outputFile, 'Log,Variance = %.20g\r\n', varValue);
        Present = varValue > G.mvf;
        if Present == false
            fprintf(outputFile, 'Log,Algorithm failed to identify peak\r\n');
        end
    end

    % 2. Choose initial parameters
    if Pass && Present
        bestN = 0;
        bestBestSigma = 0;
        bestMetric = -realmax;
        for n = find(!isnan(P) & V >= (G.rl - mod(G.rl, 3)) & V <= G.ru);
            bestSigma = 0;
            bestVar = realmax;
            for sigma = G.wl / 4 + (G.wu - G.wl) / 40:(G.wu - G.wl) / 40:G.wu / 4
                var = mp1_var(G.tfc, sigma, V(n), 0, max(P(n), 0), V, P);
                if var < bestVar
                    bestSigma = sigma;
                    bestVar = var;
                end
            end
            metric = max(P(n), 0)^2 * bestSigma / (1 + bestVar);
            if metric > bestMetric
                bestN = n;
                bestBestSigma = bestSigma;
                bestMetric = metric;
            end
        end
        sigma = repmat(0, [500, 28]);
        mu = repmat(0, [500, 28]);
        h = repmat(0, [500, 28]);
        alpha = repmat(0, [500, 28]);
        r = repmat(0, 500);
        sigma(1, 1) = bestBestSigma;
        mu(1, 1) = V(bestN);
        h(1, 1) = max(P(bestN), 0);
        alpha(1, 1) = 0;
        r(1) = 0;
    end
    
    % 3. Refine parameters
    if Pass && Present
        currentVar = mp1_var(G.tfc, sigma(1, 1), mu(1, 1), alpha(1, 1), h(1, 1), V, P);
        delta = repmat(0, [28, 1]);
        for j = 1:28
            if j >= 1 && j <= 7
                delta(j) = sigma(1, 1) / (20 * 2^(j - 1));
            elseif j >= 8 && j <= 14
                delta(j) = sigma(1, 1) / (20 * 2^(j - 8));
            elseif j >= 15 && j <= 21
                delta(j) = h(1, 1) / (100 * 2^(j - 15));
            elseif j >= 22 && j <= 28
                delta(j) =  1 / (5 * 2^(j - 22));
            end
        end
        for k = 1:499
            for j = 1:28
                if j >= 1 && j <= 7
                    [sigmaPos, muPos, alphaPos, hPos] = mp1_inc_sigma(sigma(k, j), mu(k, j), alpha(k, j), h(k, j), delta(j));
                    [sigmaNeg, muNeg, alphaNeg, hNeg] = mp1_inc_sigma(sigma(k, j), mu(k, j), alpha(k, j), h(k, j), -delta(j));
                elseif j >= 8 && j <= 14
                    [sigmaPos, muPos, alphaPos, hPos] = mp1_inc_mu(sigma(k, j), mu(k, j), alpha(k, j), h(k, j), delta(j));
                    [sigmaNeg, muNeg, alphaNeg, hNeg] = mp1_inc_mu(sigma(k, j), mu(k, j), alpha(k, j), h(k, j), -delta(j));
                elseif j >= 15 && j <= 21
                    [sigmaPos, muPos, alphaPos, hPos] = mp1_inc_h(sigma(k, j), mu(k, j), alpha(k, j), h(k, j), delta(j));
                    [sigmaNeg, muNeg, alphaNeg, hNeg] = mp1_inc_h(sigma(k, j), mu(k, j), alpha(k, j), h(k, j), -delta(j));
                elseif j >= 22 && j <= 28
                    [sigmaPos, muPos, alphaPos, hPos] = mp1_inc_alpha(sigma(k, j), mu(k, j), alpha(k, j), h(k, j), delta(j));
                    [sigmaNeg, muNeg, alphaNeg, hNeg] = mp1_inc_alpha(sigma(k, j), mu(k, j), alpha(k, j), h(k, j), -delta(j));
                end
                varPos = mp1_var(G.tfc, sigmaPos, muPos, alphaPos, hPos, V, P);
                varNeg = mp1_var(G.tfc, sigmaNeg, muNeg, alphaNeg, hNeg, V, P);
                if varPos < varNeg && varPos < currentVar
                    sigma(k, j + 1) = sigmaPos;
                    mu(k, j + 1) = muPos;
                    alpha(k, j + 1) = alphaPos;
                    h(k, j + 1) = hPos;
                    currentVar = varPos;
                elseif varNeg < varPos && varNeg < currentVar
                    sigma(k, j + 1) = sigmaNeg;
                    mu(k, j + 1) = muNeg;
                    alpha(k, j + 1) = alphaNeg;
                    h(k, j + 1) = hNeg;
                    currentVar = varNeg;
                else
                    sigma(k, j + 1) = sigma(k, j);
                    mu(k, j + 1) = mu(k, j);
                    alpha(k, j + 1) = alpha(k, j);
                    h(k, j + 1) = h(k, j);
                end
            end
            if (r(k) >= 2)
                sigma(k + 1, 1) = sigma(k, 1);
                mu(k + 1, 1) = mu(k, 1);
                alpha(k + 1, 1) = alpha(k, 1);
                h(k + 1, 1) = h(k, 1);
            else
                sigma(k + 1, 1) = sigma(k, 29);
                mu(k + 1, 1) = mu(k, 29);
                alpha(k + 1, 1) = alpha(k, 29);
                h(k + 1, 1) = h(k, 29);
            end
            prevVar = mp1_var(G.tfc, sigma(k, 1), mu(k, 1), alpha(k, 1), h(k, 1), V, P);
            var = mp1_var(G.tfc, sigma(k + 1, 1), mu(k + 1, 1), alpha(k + 1, 1), h(k + 1, 1), V, P);
            if var == 0
                r(k + 1) = 2;
            elseif isnan(G.msfh) && abs(var - prevVar) < 0.1
                r(k + 1) = r(k) + 1;
            elseif !isnan(G.msfh) && k > 10 && var == prevVar
                r(k + 1) = r(k) + 1;
            elseif !isnan(G.msfh) && k > 10 && var < max(G.mvf, (G.msfh * h(k + 1, 1))^2)
                r(k + 1) = r(k) + 1;
            else
                r(k + 1) = 0;
            end
            if r(k + 1) == 2
                fprintf(outputFile, 'Log,Algorithm converged after %d iterations with variance of %.20g\r\n', k, var);
                sigma(500, 1) = sigma(k + 1, 1);
                mu(500, 1) = mu(k + 1, 1);
                alpha(500, 1) = alpha(k + 1, 1);
                h(500, 1) = h(k + 1, 1);
                r(500) = r(k + 1);
                break
            else
                fprintf(outputFile, 'Log,Variance is %.20g after iteration %d\r\n', var, k);
            end
        end
    end
    
    % 4. Check fit convergence
    if Pass && Present
        Pass = r(500) >= 2;
        if Pass == false
            fprintf(outputFile, 'Log,Algorithm failed to converge\r\n');
        end
    end
    
    % 4. Check fit quality
    if Pass && Present
        var = mp1_var(G.tfc, sigma(500, 1), mu(500, 1), alpha(500, 1), h(500, 1), V, P);
        maxVar = G.mvf;
        if !isnan(G.msfh)
            maxVar = max(maxVar, (G.msfh * h(500, 1))^2);
        end
        Pass = var < maxVar;
        if Pass == false
            fprintf(outputFile, 'Log,Variance for peak of %.20g is more than the maximum allowed of %.20g\r\n', var, maxVar);
        end
    end
    
    % 6. Set fit result
    if Pass && Present
        F.sigma = sigma(500, 1);
        F.mu = mu(500, 1);
        F.h = h(500, 1);
        F.alpha = alpha(500, 1);
        F.p = mp1_sqp(sigma(500, 1), mu(500, 1), alpha(500, 1));
        F.var = mp1_var(G.tfc, sigma(500, 1), mu(500, 1), alpha(500, 1), h(500, 1), V, P);
    end
end


function [flatLine, Fs] = mp1_analyse(outputFile, Tmvd, Tmvp, Tmvpc, D, Gs, V, C)
    
    flatLine = false;
    Fs = repmat(struct('oc', 'False', 'sigma', 0.0, 'mu', 0.0, 'h', 0.0, 'alpha', 0.0, 'p', 0.0, 'var', 0.0), [size(Gs)(1), 1]);
    
    Pass = true;
    
    mp1_writesignal(outputFile, 'Raw Data', V, C);

    % 1. Perform flat line data check
    if Pass
        flatLine = max(abs(C)) > 50;
        if flatLine == false
            fprintf(outputFile, 'Log,Raw data is flat-line\r\n');
        end
    end

    % 2. Perform Savitsky Golay smoothing with width 17 and degree 2
    if Pass
        fprintf(outputFile, 'Log,SavitskyGolay(17, 2)\r\n');
        SG = [-21, -6, 7, 18, 27, 34, 39, 42, 43, 42, 39, 34, 27, 18, 7, -6, -21];
        SG = SG ./ sum(SG);
        S = repmat(NaN, size(C));
        for n = 9:size(C)(2) - 8
            S(n) = sum(C(n - 8:n + 8) .* SG);
        end
        mp1_writesignal(outputFile, 'Smoothed Data', V, S);
     end
     
    % 3. Perform variance check
    if Pass
        varValue = 0.0;
        varCount = 0;
        for n = 1:size(S)(2)
            if !isnan(S(n))
                varValue += (S(n) - C(n))^2;
                ++varCount;
            end
        end
        varValue = varValue / varCount;
        Pass = varValue < Tmvd;
        if Pass
            fprintf(outputFile, 'Log,Variance for raw data is %.20g\r\n', varValue);
        else
            fprintf(outputFile, 'Log,Variance for raw data of %.20g is more than the maximum of %.20g\r\n', varValue, Tmvd);
        end
    end
                    
    % 4. Perform baseline estimation
    if Pass
        L = repmat(NaN, [100, size(S)(2)]);
        bl = repmat(0, [100, D + 1]);
        L(1, :) = C(:);
        fprintf(outputFile, 'Log,CurveFit(%d)\r\n', D);
        bl(1, :) = mp1_polyfit(V, L(1, :), D);
        inRange = repmat(0, size(V));
        for g = 1:size(Gs)(1)
            inRange = inRange | (V >= Gs(g).rl & V <= Gs(g).ru);
        end
        for k = 1:99
            includeIndexes = find(not(inRange & (C .- polyval(bl(k, :), V)) >= sqrt(Tmvp)));
            L(k + 1, includeIndexes) = C(includeIndexes);
            if sum(!isnan(L(k + 1, :))) != sum(!isnan(L(k, :)))
                fprintf(outputFile, 'Log,CurveFit(%d)\r\n', D);
                bl(k + 1, :) = mp1_polyfit(V, L(k + 1, :), D);
            else
                bl(k + 1, :) = bl(k, :);
            end
        end
    end

    % 5. Check polynomial convergence
    if Pass
        Pass = sum(!isnan(L(100, :))) == sum(!isnan(L(99, :)));
        if Pass == false
            fprintf(outputFile, 'Log,Failed to fit a polynomial to the data\r\n');
        end
        mp1_writesignal(outputFile, 'Baseline Fitting Data', V, L(100, :));
        mp1_writesignal(outputFile, 'Baseline Estimation', V, polyval(bl(100, :), V));
    end

    % 6. Check quality of polynomial fit
    if Pass
        varValue = 0.0;
        varCount = 0;
        for n = 1:size(C)(2)
            if !isnan(L(100, n))
                varValue += (C(n) - polyval(bl(100, :), V(n)))^2;
                ++varCount;
            end
        end
        varValue = varValue / varCount;
        Pass = varValue < Tmvpc;
        if Pass
            fprintf(outputFile, 'Log,Variance for polynomial is %.20g\r\n', varValue);
        else
            fprintf(outputFile, 'Log,Variance for polynomial of %.20g is more than the maximum of %.20g\r\n', varValue, Tmvpc);
        end
    end

    % 7. Remove polynomial
    if Pass
        P = S;
        for n = 1:size(P)(2)
            if !isnan(P(n))
               P(n) = S(n) -  polyval(bl(100, :), V(n));
            end
        end
        mp1_writesignal(outputFile, 'Issolated Peaks', V, P);
    end

    % 8 Fit peaks & 9 Determine peak outcome
    for g = 1:size(Gs)(1)
        if Gs(g).fit
            if Pass
                [fitOk, F] = mp1_gaussfit(outputFile, Gs(g), V, P);
                Fs(g) = F;
                if fitOk
                    if Fs(g).h >= Gs(g).hl && Fs(g).h <= Gs(g).hu && Fs(g).p >= Gs(g).mu - Gs(g).mut && Fs(g).p <= Gs(g).mu + Gs(g).mut && 4 * Fs(g).sigma >= Gs(g).wl && 4 * Fs(g).sigma <= Gs(g).wu
                        fprintf(outputFile, 'Log,Peak %s is positve\r\n', Gs(g).name);
                        Fs(g).oc = 'Positive';
                    else
                        fprintf(outputFile, 'Log,Peak %s is negative\r\n', Gs(g).name);
                        Fs(g).oc = 'Negative';
                    end
                end
            end
        else
            fprintf(outputFile, 'Log,Peak %s is ignored\r\n', Gs(g).name);
            Fs(g).oc = 'Ignore';
        end
    end
end


%------------------------------------------------------------------
%                               Script
%------------------------------------------------------------------
% Get input data
[directory, name, ext] = fileparts(mfilename('fullpath'));
parameters = dlmread(fullfile(directory, 'parameters.csv'), ',', [0, 0, 0, 3]);
Tmvd = parameters(1);
Tmvpc = parameters(2);
D = parameters(3);
Tmvp = parameters(4);
signal = dlmread(fullfile(directory, 'signal.csv'), ',');
V = signal(1, :);
C = signal(2, :);
lines = importdata(fullfile(directory, 'peaks.csv'));
if size(lines)(2) > 0
    Gs = repmat(struct('name', '', 'fit', false, 'rl', NaN, 'ru', NaN, 'mu', NaN, 'mut', NaN, 'hl', NaN, 'hu', NaN, 'wl', NaN, 'wu', NaN, 'mvf', NaN, 'tfc', NaN, 'msfh', NaN), [size(lines)(1), 1]);
    for g = 1:size(lines)(1)
        % Gauss are passed as: Type,Name,Potentiostat,MinPotential,MaxPotential,Mean,Tolerance,LowerLimit,UpperLimit,MaxVarianceForCurveFit,TopFractionForCurveFit,MaxWidth,MinWidth,MaxSdAsFractionOfHeight
        tokens = textscan(lines{g}, '%s,%s,%f,%f,%f,%f,%f,%f,%f,%f,%f,%f,%f,%f', 'Delimiter', ',');
        Gs(g).name = tokens{1, 2}{1};
        Gs(g).fit = not(strcmp(tokens{1, 1}{1}, 'Ignore'));
        Gs(g).rl = tokens{1, 4};
        Gs(g).ru = tokens{1, 5};
        Gs(g).mu = tokens{1, 6};
        Gs(g).mut = tokens{1, 7};
        Gs(g).hl = tokens{1, 8};
        Gs(g).hu = tokens{1, 9};
        Gs(g).wl = tokens{1, 13};
        Gs(g).wu = tokens{1, 12};
        Gs(g).mvf = tokens{1, 10};
        Gs(g).tfc = tokens{1, 11};
        Gs(g).msfh = tokens{1, 14};
    end
else
   Gs = repmat(struct('name', '', 'fit', false, 'rl', NaN, 'ru', NaN, 'mu', NaN, 'mut', NaN, 'hl', NaN, 'hu', NaN, 'wl', NaN, 'wu', NaN, 'mvf', NaN, 'tfc', NaN, 'msfh', NaN), [0, 0]); 
end

% Create output file
outputFile = fopen(fullfile(directory, 'output.csv'), 'w');    

% Run algorithm
[flatLine, Fs] = mp1_analyse(outputFile, Tmvd, Tmvp, Tmvpc, D, Gs, V, C);

% Output flatline
if flatLine
    fprintf(outputFile, 'FlatLine\r\n');
end

% Output fitted Gaussian
for g = 1:size(Gs)(1)
    if Gs(g).fit
        fprintf(outputFile, 'Peak,%s,%.20g,%.20g,%.20g,%.20g,%.20g,%s\r\n', Gs(g).name, Fs(g).h, Fs(g).p, Fs(g).sigma * 4, Fs(g).alpha, Fs(g).var, Fs(g).oc);
    end
end

% Close output file
fclose(outputFile);